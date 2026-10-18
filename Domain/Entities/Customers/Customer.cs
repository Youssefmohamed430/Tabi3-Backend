using Domain.Enums;
using Domain.Entities.Common;

namespace Domain.Customers;

public sealed partial class Customer : TenantEntity
{
    
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string NormalizedPhone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset? WhatsAppOptInAt { get; set; }
    public string? PortalUserId { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public DateTimeOffset? MarketingOptInAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


