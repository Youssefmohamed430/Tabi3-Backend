namespace Application.Dtos.Auth;

public sealed record LoginRequestDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public Guid? TenantId { get; init; }
}
