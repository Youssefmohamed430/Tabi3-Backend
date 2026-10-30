namespace Application.Services;

public sealed class MemberService(
    IRepository<TenantMembership> memberships,
    IRepository<TenantRole> tenantRoles,
    IUnitOfWork unitOfWork,
    ICurrentTenant currentTenant,
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IMemberService
{
    public async Task<Result<IReadOnlyList<TenantMembership>>> GetMembersAsync(CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return Result.Failure<IReadOnlyList<TenantMembership>>(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));
        if (!CanManageMembers())
            return Result.Failure<IReadOnlyList<TenantMembership>>(new Error("Membership.Forbidden", "لا تملك صلاحية إدارة أعضاء المطبعة."));

        var result = await memberships.ListAsync(new MembersByTenantSpecification(tenantId), cancellationToken);
        return Result.Success(result);
    }

    public async Task<Result<TenantMembership>> InviteAsync(InviteMemberRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return Result.Failure<TenantMembership>(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));
        if (!CanManageMembers())
            return Result.Failure<TenantMembership>(new Error("Membership.Forbidden", "لا تملك صلاحية إدارة أعضاء المطبعة."));
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.RoleCode))
            return Result.Failure<TenantMembership>(new Error("Membership.InvalidRequest", "البريد الإلكتروني والدور مطلوبان."));

        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Result.Failure<TenantMembership>(new Error("Membership.UserNotFound", "لا يوجد حساب مسجل بهذا البريد. يجب إنشاء الحساب قبل إضافته للمطبعة."));

        var role = await tenantRoles.FirstOrDefaultAsync(new TenantRoleByCodeSpecification(request.RoleCode.Trim()), cancellationToken);
        if (role is null)
            return Result.Failure<TenantMembership>(new Error("Membership.RoleNotFound", "الدور المطلوب غير معرف في النظام."));

        if (string.Equals(role.Code, "SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role.Code, "Customer", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<TenantMembership>(new Error("Membership.InvalidRole", "لا يمكن إسناد هذا الدور كعضوية موظف في المطبعة."));

        var existing = await memberships.FirstOrDefaultAsync(new MembershipByUserSpecification(tenantId, user.Id), cancellationToken);
        if (existing is not null)
            return Result.Failure<TenantMembership>(new Error("Membership.AlreadyExists", "هذا المستخدم مرتبط بالمطبعة بالفعل."));

        var actorId = GetCurrentUserId();
        if (actorId is null)
            return Result.Failure<TenantMembership>(new Error("Membership.ActorRequired", "تعذر تحديد المستخدم الحالي."));

        var membership = new TenantMembership
        {
            TenantId = tenantId,
            UserId = user.Id,
            RoleId = role.Id,
            Status = MembershipStatus.Active,
            PermissionsJson = SerializePermissions(request.Permissions),
            InvitedByUserId = actorId,
            JoinedAt = EgyptianDateTime.Now,
            CreatedAt = EgyptianDateTime.Now
        };

        await memberships.AddAsync(membership, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(membership, "تمت إضافة العضو إلى المطبعة.");
    }

    public async Task<Result<TenantMembership>> UpdateMembershipAsync(UpdateMemberRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return Result.Failure<TenantMembership>(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));
        if (!CanManageMembers())
            return Result.Failure<TenantMembership>(new Error("Membership.Forbidden", "لا تملك صلاحية إدارة أعضاء المطبعة."));

        var membership = await memberships.FirstOrDefaultAsync(new MembershipByTenantAndIdSpecification(tenantId, request.MembershipId), cancellationToken);
        if (membership is null)
            return Result.Failure<TenantMembership>(new Error("Membership.NotFound", "العضوية غير موجودة في المطبعة الحالية."));

        var role = await tenantRoles.FirstOrDefaultAsync(new TenantRoleByCodeSpecification(request.RoleCode.Trim()), cancellationToken);
        if (role is null)
            return Result.Failure<TenantMembership>(new Error("Membership.RoleNotFound", "الدور المطلوب غير معرف في النظام."));
        if (string.Equals(role.Code, "SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(role.Code, "Customer", StringComparison.OrdinalIgnoreCase))
            return Result.Failure<TenantMembership>(new Error("Membership.InvalidRole", "لا يمكن إسناد هذا الدور كعضوية موظف في المطبعة."));

        membership.RoleId = role.Id;
        membership.PermissionsJson = SerializePermissions(request.Permissions);
        memberships.Update(membership);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(membership, "تم تحديث عضوية المستخدم.");
    }

    public async Task<Result> SetMembershipActiveAsync(Guid membershipId, bool isActive, CancellationToken cancellationToken = default)
    {
        if (!TryGetTenantId(out var tenantId))
            return Result.Failure(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));
        if (!CanManageMembers())
            return Result.Failure(new Error("Membership.Forbidden", "لا تملك صلاحية إدارة أعضاء المطبعة."));

        var membership = await memberships.FirstOrDefaultAsync(new MembershipByTenantAndIdSpecification(tenantId, membershipId), cancellationToken);
        if (membership is null)
            return Result.Failure(new Error("Membership.NotFound", "العضوية غير موجودة في المطبعة الحالية."));

        if (membership.Role.Code.Equals("Owner", StringComparison.OrdinalIgnoreCase) && !isActive)
            return Result.Failure(new Error("Membership.OwnerCannotBeDeactivated", "لا يمكن إيقاف عضوية المالك بهذه العملية."));

        membership.Status = isActive ? MembershipStatus.Active : MembershipStatus.Inactive;
        if (isActive && membership.JoinedAt is null)
            membership.JoinedAt = EgyptianDateTime.Now;
        memberships.Update(membership);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(isActive ? "تم تفعيل العضوية." : "تم إيقاف العضوية.");
    }

    private bool TryGetTenantId(out Guid tenantId)
    {
        tenantId = currentTenant.TenantId.GetValueOrDefault();
        return tenantId != Guid.Empty;
    }

    private bool CanManageMembers()
    {
        var role = httpContextAccessor.HttpContext?.User.FindFirst("tenant_role")?.Value;
        return string.Equals(role, "Owner", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);
    }

    private string? GetCurrentUserId() =>
        httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
        httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

    private static string? SerializePermissions(IReadOnlyCollection<string>? permissions)
    {
        if (permissions is null || permissions.Count == 0)
            return null;
        var normalized = permissions.Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToArray();
        return normalized.Length == 0 ? null : System.Text.Json.JsonSerializer.Serialize(normalized);
    }

    private sealed class MembersByTenantSpecification : Specification<TenantMembership>
    {
        public MembersByTenantSpecification(Guid tenantId) => Query
            .Where(item => item.TenantId == tenantId)
            .Include(item => item.User)
            .Include(item => item.Role)
            .OrderBy(item => item.CreatedAt);
    }

    private sealed class MembershipByUserSpecification : Specification<TenantMembership>
    {
        public MembershipByUserSpecification(Guid tenantId, string userId) => Query
            .Where(item => item.TenantId == tenantId && item.UserId == userId);
    }

    private sealed class MembershipByTenantAndIdSpecification : Specification<TenantMembership>
    {
        public MembershipByTenantAndIdSpecification(Guid tenantId, Guid membershipId) => Query
            .Where(item => item.TenantId == tenantId && item.Id == membershipId)
            .Include(item => item.Role);
    }

    private sealed class TenantRoleByCodeSpecification : Specification<TenantRole>
    {
        public TenantRoleByCodeSpecification(string roleCode) => Query
            .Where(role => role.Code == roleCode);
    }
}
