using Domain.Entities.Common;
using Domain.ValueObjects;

namespace Domain.Subscriptions;

public sealed partial class SaaSPlan : Entity
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public Price MonthlyPrice { get; set; } = Price.Zero;
    public Price YearlyPrice { get; set; } = Price.Zero;
    public int? MaxStaffUsers { get; set; }
    public int? MaxOrdersPerMonth { get; set; }
    public long? MaxStorageBytes { get; set; }
    public int? MaxPrintDevices { get; set; }
    public int? MaxWhatsAppNumbers { get; set; }
    public bool IncludesCustomerPortal { get; set; }
    public bool IncludesPublicWebsite { get; set; }
    public bool IncludesAdvancedReports { get; set; }
    public bool IncludesExtendedEventHistory { get; set; }
    public bool IncludesPrioritySupport { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


