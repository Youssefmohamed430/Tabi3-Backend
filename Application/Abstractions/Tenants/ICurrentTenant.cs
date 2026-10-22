namespace Application.Abstractions.Tenants;

public interface ICurrentTenant
{
    Guid? TenantId { get; }
    bool IsPlatformAdministrator { get; }
}
