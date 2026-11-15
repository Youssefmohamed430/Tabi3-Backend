namespace Application.Dtos.Auth;

public sealed record RegisterOwnerRequestDto
{
    public required string OwnerName { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required string Password { get; init; }
    public required string BusinessName { get; init; }
    public required string TenantSlug { get; init; }
}
