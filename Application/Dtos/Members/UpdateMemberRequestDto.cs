namespace Application.Dtos.Members;

public sealed record UpdateMemberRequestDto
{
    public required Guid MembershipId { get; init; }
    public required string RoleCode { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
