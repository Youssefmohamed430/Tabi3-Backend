namespace Application.Specifications.Tenants;

public sealed class TenantByIdSpecification : Specification<Tenant>
{
    public TenantByIdSpecification(Guid tenantId) => Query.Where(item => item.Id == tenantId);
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
