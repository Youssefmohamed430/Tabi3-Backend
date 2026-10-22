namespace Application.Dtos.Members;

public sealed record InviteMemberRequestDto
{
    public required string Email { get; init; }
    public required string RoleCode { get; init; }
    public IReadOnlyCollection<string> Permissions { get; init; } = [];
}
