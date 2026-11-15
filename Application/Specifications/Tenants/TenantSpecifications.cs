namespace Application.Specifications.Tenants;

public sealed class TenantByIdSpecification : Specification<Tenant>
{
    public TenantByIdSpecification(Guid tenantId) => Query.Where(item => item.Id == tenantId);
}

public sealed class TenantBySlugSpecification : Specification<Tenant>
{
    public TenantBySlugSpecification(string slug) => Query.Where(item => item.Slug == slug);
}

public sealed class TenantRegistrationBySlugSpecification : Specification<TenantRegistrationRequest>
{
    public TenantRegistrationBySlugSpecification(string slug) => Query.Where(item => item.Slug == slug && !item.IsDeleted);
}

public sealed class PendingTenantRegistrationByOwnerSpecification : Specification<TenantRegistrationRequest>
{
    public PendingTenantRegistrationByOwnerSpecification(string ownerUserId) => Query
        .Where(item => item.OwnerUserId == ownerUserId &&
                       (item.Status == TenantRegistrationStatus.Submitted || item.Status == TenantRegistrationStatus.PendingReview));
}

public sealed class PendingTenantRegistrationRequestsSpecification : Specification<TenantRegistrationRequest>
{
    public PendingTenantRegistrationRequestsSpecification() => Query
        .Where(item => item.Status == TenantRegistrationStatus.Submitted || item.Status == TenantRegistrationStatus.PendingReview)
        .OrderBy(item => item.CreatedAt);
}

public sealed class TenantRegistrationRequestByIdSpecification : Specification<TenantRegistrationRequest>
{
    public TenantRegistrationRequestByIdSpecification(Guid registrationRequestId) => Query
        .Where(item => item.Id == registrationRequestId);
}
