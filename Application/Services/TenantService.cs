namespace Application.Services;

public sealed class TenantService(
    IRepository<Tenant> tenants,
    IRepository<TenantRegistrationRequest> registrationRequests,
    IUnitOfWork unitOfWork,
    ICurrentTenant currentTenant,
    IHttpContextAccessor httpContextAccessor) : ITenantService
{
    public async Task<Result<Tenant>> GetCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        if (currentTenant.TenantId is not Guid tenantId)
            return Result.Failure<Tenant>(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));

        var tenant = await tenants.FirstOrDefaultAsync(new TenantByIdSpecification(tenantId), cancellationToken);

        return tenant is null
            ? Result.Failure<Tenant>(new Error("Tenant.NotFound", "المطبعة غير موجودة أو غير متاحة."))
            : Result.Success(tenant);
    }

    public async Task<Result<Tenant>> UpdateCurrentTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        if (tenant is null)
            return Result.Failure<Tenant>(new Error("Tenant.Invalid", "بيانات المطبعة مطلوبة."));

        if (currentTenant.TenantId is not Guid tenantId)
            return Result.Failure<Tenant>(new Error("Tenant.ContextRequired", "يلزم تحديد المطبعة الحالية."));

        var existingTenant = await tenants.FirstOrDefaultAsync(new TenantByIdSpecification(tenantId), cancellationToken);
        if (existingTenant is null)
            return Result.Failure<Tenant>(new Error("Tenant.NotFound", "المطبعة غير موجودة أو غير متاحة."));

        if (string.IsNullOrWhiteSpace(tenant.Name) || tenant.Name.Trim().Length > 200)
            return Result.Failure<Tenant>(new Error("Tenant.InvalidName", "اسم المطبعة مطلوب وبحد أقصى 200 حرف."));

        existingTenant.Name = tenant.Name.Trim();
        existingTenant.BusinessName = NormalizeOptional(tenant.BusinessName, 200);
        existingTenant.PublicDescription = NormalizeOptional(tenant.PublicDescription, 2000);
        existingTenant.Phone = NormalizeOptional(tenant.Phone, 32);
        existingTenant.Email = NormalizeOptional(tenant.Email, 254);
        existingTenant.Address = NormalizeOptional(tenant.Address, 500);
        existingTenant.LogoObjectKey = NormalizeOptional(tenant.LogoObjectKey, 1024);
        existingTenant.PrimaryColor = NormalizeColor(tenant.PrimaryColor, "#142D4E");
        existingTenant.SecondaryColor = NormalizeColor(tenant.SecondaryColor, "#17A6A1");
        existingTenant.AccentColor = NormalizeColor(tenant.AccentColor, "#F2A541");
        existingTenant.BackgroundColor = NormalizeColor(tenant.BackgroundColor, "#F7F5F0");
        existingTenant.UpdatedAt = EgyptianDateTime.Now;

        tenants.Update(existingTenant);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(existingTenant, "تم تحديث بيانات المطبعة.");
    }

    public async Task<Result<IReadOnlyList<TenantRegistrationRequest>>> GetRegistrationRequestsAsync(CancellationToken cancellationToken = default)
    {
        if (!IsPlatformAdministrator())
            return Result.Failure<IReadOnlyList<TenantRegistrationRequest>>(new Error("Tenant.Forbidden", "هذه العملية متاحة لمسؤول المنصة فقط."));

        var requests = await registrationRequests.ListAsync(new PendingTenantRegistrationRequestsSpecification(), cancellationToken);
        return Result.Success(requests);
    }

    public async Task<Result<Tenant>> ReviewRegistrationAsync(ReviewTenantRegistrationRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!IsPlatformAdministrator())
            return Result.Failure<Tenant>(new Error("Tenant.Forbidden", "هذه العملية متاحة لمسؤول المنصة فقط."));

        var registration = await registrationRequests.FirstOrDefaultAsync(
            new TenantRegistrationRequestByIdSpecification(request.RegistrationRequestId), cancellationToken);
        if (registration is null)
            return Result.Failure<Tenant>(new Error("Tenant.RegistrationNotFound", "طلب تسجيل المطبعة غير موجود."));

        if (registration.Status is not (TenantRegistrationStatus.Submitted or TenantRegistrationStatus.PendingReview))
            return Result.Failure<Tenant>(new Error("Tenant.RegistrationAlreadyReviewed", "سبق اتخاذ قرار بشأن هذا الطلب."));

        var actorId = httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(actorId))
            return Result.Failure<Tenant>(new Error("Tenant.ActorRequired", "تعذر تحديد مسؤول المنصة الحالي."));

        var now = EgyptianDateTime.Now;
        registration.DecidedByUserId = actorId;
        registration.DecidedAt = now;
        registration.DecisionReason = NormalizeOptional(request.DecisionReason, 1000);
        registration.Status = request.IsApproved ? TenantRegistrationStatus.Approved : TenantRegistrationStatus.Rejected;
        registrationRequests.Update(registration);

        Tenant? createdTenant = null;
        if (request.IsApproved)
        {
            if (string.IsNullOrWhiteSpace(registration.OwnerUserId) || string.IsNullOrWhiteSpace(registration.Slug))
                return Result.Failure<Tenant>(new Error("Tenant.RegistrationInvalid", "طلب التسجيل يفتقد بيانات المالك أو الرابط."));

            createdTenant = new Tenant
            {
                Name = registration.BusinessName,
                BusinessName = registration.BusinessName,
                Slug = registration.Slug,
                Phone = registration.Phone,
                Email = registration.Email,
                OwnerUserId = registration.OwnerUserId,
                Status = TenantStatus.Active,
                ApprovedAt = now,
                ApprovedByUserId = actorId,
                TrialStartedAt = request.StartTrial ? now : null,
                TrialEndsAt = request.StartTrial ? now.AddDays(14) : null,
                CreatedAt = now,
                UpdatedAt = now
            };
            await tenants.AddAsync(createdTenant, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return request.IsApproved
            ? Result.Success(createdTenant!, "تم اعتماد طلب المطبعة.")
            : Result.Success<Tenant>(null!, registration.DecisionReason ?? "تم رفض طلب تسجيل المطبعة.");
    }

    public async Task<Result<Tenant>> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        if (!IsPlatformAdministrator())
            return Result.Failure<Tenant>(new Error("Tenant.Forbidden", "هذه العملية متاحة لمسؤول المنصة فقط."));

        var tenant = await tenants.FirstOrDefaultAsync(new TenantByIdSpecification(tenantId), cancellationToken);
        if (tenant is null)
            return Result.Failure<Tenant>(new Error("Tenant.NotFound", "المطبعة غير موجودة."));
        if (tenant.Status == TenantStatus.Active)
            return Result.Success(tenant, "المطبعة مفعلة بالفعل.");

        tenant.Status = TenantStatus.Active;
        tenant.UpdatedAt = EgyptianDateTime.Now;
        tenants.Update(tenant);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(tenant, "تم تفعيل المطبعة.");
    }

    private bool IsPlatformAdministrator() =>
        httpContextAccessor.HttpContext?.User.IsInRole(DomainConstants.IdentityRoles.SuperAdmin) == true &&
        currentTenant.IsPlatformAdministrator;

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim();
        return normalized.Length <= maxLength ? normalized : null;
    }

    private static string NormalizeColor(string? value, string fallback) =>
        !string.IsNullOrWhiteSpace(value) && System.Text.RegularExpressions.Regex.IsMatch(value, "^#[0-9A-Fa-f]{6}$")
            ? value.ToUpperInvariant()
            : fallback;

    private sealed class TenantByIdSpecification : Specification<Tenant>
    {
        public TenantByIdSpecification(Guid tenantId) => Query.Where(item => item.Id == tenantId);
    }

    private sealed class PendingTenantRegistrationRequestsSpecification : Specification<TenantRegistrationRequest>
    {
        public PendingTenantRegistrationRequestsSpecification() => Query
            .Where(item => item.Status == TenantRegistrationStatus.Submitted || item.Status == TenantRegistrationStatus.PendingReview)
            .OrderBy(item => item.CreatedAt);
    }

    private sealed class TenantRegistrationRequestByIdSpecification : Specification<TenantRegistrationRequest>
    {
        public TenantRegistrationRequestByIdSpecification(Guid registrationRequestId) => Query
            .Where(item => item.Id == registrationRequestId);
    }
}
