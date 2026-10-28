namespace Application.Services;

internal class TenantService : ITenantService
{
    public Task<Result<Tenant>> ActivateTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Tenant>> GetCurrentTenantAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IReadOnlyList<TenantRegistrationRequest>>> GetRegistrationRequestsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Tenant>> ReviewRegistrationAsync(ReviewTenantRegistrationRequestDto request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Tenant>> UpdateCurrentTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
