namespace Infrastructure.Identity;

internal sealed class HttpCurrentTenant(IHttpContextAccessor httpContextAccessor) : ICurrentTenant
{
    public Guid? TenantId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
            return Guid.TryParse(value, out var tenantId) ? tenantId : null;
        }
    }

    public bool IsPlatformAdministrator
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            return user?.Identity?.IsAuthenticated == true &&
                   !user.HasClaim(claim => claim.Type == "tenant_id") &&
                   user.IsInRole("SuperAdmin");
        }
    }
}
