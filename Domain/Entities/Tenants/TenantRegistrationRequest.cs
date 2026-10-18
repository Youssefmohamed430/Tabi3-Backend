using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.Tenants;

public sealed partial class TenantRegistrationRequest : Entity, ISoftDeletable
{
    public string BusinessName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string OwnerUserId { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public TenantRegistrationStatus Status { get; set; }
    public string? DecisionReason { get; set; }
    public string? DecidedByUserId { get; set; }
    public DateTimeOffset? DecidedAt { get; set; }
    public string? ActivationMode { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public bool IsDeleted { get; set; }
}


