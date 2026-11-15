namespace Application.Specifications.Identity;

public sealed class ActiveMembershipsByUserSpecification : Specification<TenantMembership>
{
    public ActiveMembershipsByUserSpecification(string userId) => Query
        .IgnoreQueryFilters()
        .Where(item => item.UserId == userId && item.Status == MembershipStatus.Active && !item.IsDeleted)
        .Include(item => item.Tenant)
        .Include(item => item.Role);
}

public sealed class ActiveMembershipByIdAndUserSpecification : Specification<TenantMembership>
{
    public ActiveMembershipByIdAndUserSpecification(Guid membershipId, string userId) => Query
        .IgnoreQueryFilters()
        .Where(item => item.Id == membershipId && item.UserId == userId && item.Status == MembershipStatus.Active && !item.IsDeleted)
        .Include(item => item.Tenant)
        .Include(item => item.Role);
}

public sealed class ActiveCustomerAccountsByUserSpecification : Specification<Domain.Customers.Customer>
{
    public ActiveCustomerAccountsByUserSpecification(string userId) => Query
        .IgnoreQueryFilters()
        .Where(item => item.PortalUserId == userId && item.IsActive && !item.IsDeleted)
        .Include(item => item.Tenant);
}
