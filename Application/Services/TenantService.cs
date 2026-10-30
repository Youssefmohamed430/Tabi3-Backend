namespace Application.Services;

using Application.Specifications.Tenants;

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

        tenant.Adapt(existingTenant);

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

            createdTenant = registration.Adapt<Tenant>();
            createdTenant.Status = TenantStatus.Active;
            createdTenant.ApprovedAt = now;
            createdTenant.ApprovedByUserId = actorId;
            createdTenant.TrialStartedAt = request.StartTrial ? now : null;
            createdTenant.TrialEndsAt = request.StartTrial ? now.AddDays(14) : null;
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

}
