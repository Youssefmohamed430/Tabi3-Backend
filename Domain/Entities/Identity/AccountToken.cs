namespace Domain.Identity;

public sealed partial class AccountToken : Entity, ISoftDeletable
{
    public string UserId { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public Guid? MembershipId { get; set; }
    public AccountTokenPurpose Purpose { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? ConsumedAt { get; set; }
    public string? MetadataJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public bool IsDeleted { get; set; }
}


