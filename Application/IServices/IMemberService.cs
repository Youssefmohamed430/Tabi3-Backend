namespace Application.IServices;

public interface IMemberService
{
    Task<Result<TenantMembership>> InviteAsync(
        InviteMemberRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<TenantMembership>>> GetMembersAsync(
        CancellationToken cancellationToken = default);

    Task<Result<TenantMembership>> UpdateMembershipAsync(
        UpdateMemberRequestDto request,
        CancellationToken cancellationToken = default);

    Task<Result> SetMembershipActiveAsync(
        Guid membershipId,
        bool isActive,
        CancellationToken cancellationToken = default);
}
