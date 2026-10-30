namespace Application.Specifications.Members;

public sealed class MembersByTenantSpecification : Specification<TenantMembership>
{
    public MembersByTenantSpecification(Guid tenantId) => Query
        .Where(item => item.TenantId == tenantId)
        .Include(item => item.User)
        .Include(item => item.Role)
        .OrderBy(item => item.CreatedAt);
}

public sealed class MembershipByUserSpecification : Specification<TenantMembership>
{
    public MembershipByUserSpecification(Guid tenantId, string userId) => Query
        .Where(item => item.TenantId == tenantId && item.UserId == userId);
}

public sealed class MembershipByTenantAndIdSpecification : Specification<TenantMembership>
{
    public MembershipByTenantAndIdSpecification(Guid tenantId, Guid membershipId) => Query
        .Where(item => item.TenantId == tenantId && item.Id == membershipId)
        .Include(item => item.Role);
}

public sealed class TenantRoleByCodeSpecification : Specification<TenantRole>
{
    public TenantRoleByCodeSpecification(string roleCode) => Query
        .Where(role => role.Code == roleCode);
}
