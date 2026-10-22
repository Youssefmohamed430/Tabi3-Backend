namespace Application.Abstractions.Identity;

public sealed record AccessTokenResult(string Token, DateTimeOffset ExpiresAt);

public interface IAccessTokenService
{
    Task<AccessTokenResult> CreateAsync(string userId, Guid? tenantId, CancellationToken cancellationToken = default);
}
