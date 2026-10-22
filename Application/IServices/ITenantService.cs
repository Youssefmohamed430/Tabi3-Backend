namespace Application.IServices;

public interface ITenantService
{
    Task<Result<Tenant>> GetCurrentTenantAsync(CancellationToken cancellationToken = default);

    Task<Result<Tenant>> UpdateCurrentTenantAsync(
        Tenant tenant,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TenantRegistrationRequest>>> GetRegistrationRequestsAsync(
        CancellationToken cancellationToken = default);

    Task<Result<Tenant>> ReviewRegistrationAsync(
        ReviewTenantRegistrationRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Result<Tenant>> ActivateTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default);
}
