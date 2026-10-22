namespace Domain.Tenants;


public sealed partial class TenantMembership : TenantEntity
{
    
    public string UserId { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public TenantRole Role { get; set; } = null!;
    public MembershipStatus Status { get; set; }
    public string? PermissionsJson { get; set; }
    public string? InvitedByUserId { get; set; }
    public DateTimeOffset? JoinedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


