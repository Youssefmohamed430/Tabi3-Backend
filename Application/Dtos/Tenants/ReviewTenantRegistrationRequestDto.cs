namespace Application.Dtos.Tenants;

public sealed record ReviewTenantRegistrationRequestDto
{
    public required Guid RegistrationRequestId { get; init; }
    public required bool IsApproved { get; init; }
    public string? DecisionReason { get; init; }
    public bool StartTrial { get; init; }
}
