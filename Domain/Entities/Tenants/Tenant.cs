namespace Domain.Tenants;


public sealed partial class Tenant : Entity
{
    
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? LogoObjectKey { get; set; }
    public string? PrimaryColor { get; set; } = "#142D4E";
    public string? SecondaryColor { get; set; } = "#17A6A1";
    public string? AccentColor { get; set; } = "#F2A541";
    public string? BackgroundColor { get; set; } = "#F7F5F0";
    public string? PublicDescription { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string OwnerUserId { get; set; } = string.Empty;
    public string Currency { get; set; } = Domain.Constants.DomainConstants.DefaultCurrency;
    public string TimeZone { get; set; } = "Africa/Cairo";
    public TenantStatus Status { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public string? ApprovedByUserId { get; set; }
    public DateTimeOffset? TrialStartedAt { get; set; }
    public DateTimeOffset? TrialEndsAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


