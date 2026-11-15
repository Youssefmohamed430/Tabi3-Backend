namespace Application.IServices;

public interface IAuthService
{
    Task<Result<Guid>> RegisterOwnerAsync(
        RegisterOwnerRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Result<AuthModel>> LoginAsync(
        string email,
        string password,
        Guid? tenantId = null,
        CancellationToken cancellationToken = default);

    Task<Result<AuthModel>> RefreshSessionAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result<AuthModel>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    Task<Result> RequestPasswordResetAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default);

    Task<Result> VerifyEmailAsync(
        string verificationToken,
        CancellationToken cancellationToken = default);

    Task<Result<AuthModel>> AcceptInvitationAsync(
        string invitationToken,
        CancellationToken cancellationToken = default);

    Task<Result> CompleteAccountSetupAsync(
        string accountSetupToken,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthModel>> SelectTenantAsync(
        Guid membershipId,
        CancellationToken cancellationToken = default);
}
