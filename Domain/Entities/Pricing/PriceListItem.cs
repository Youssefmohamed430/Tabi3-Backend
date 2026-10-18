using Domain.Enums;
using Domain.Entities.Common;
using Domain.ValueObjects;

namespace Domain.Pricing;

public sealed partial class PriceListItem : TenantEntity
{
    
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? PaperSize { get; set; }
    public string? ColorMode { get; set; }
    public string? Sides { get; set; }
    public string? PaperType { get; set; }
    public string BillingUnit { get; set; } = string.Empty;
    public Price UnitPrice { get; set; } = Price.Zero;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset EffectiveFrom { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public string? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


