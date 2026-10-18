using Domain.Enums;
using Domain.Entities.Common;
using Domain.ValueObjects;

namespace Domain.Orders;

public sealed partial class OrderItem : TenantEntity
{
    
    public Guid OrderId { get; set; }
    public Guid? PriceListItemId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? PaperSize { get; set; }
    public string? ColorMode { get; set; }
    public string? Sides { get; set; }
    public string? PaperType { get; set; }
    public int PageCount { get; set; }
    public int CopyCount { get; set; }
    public int BillableUnits { get; set; }
    public Price UnitPrice { get; set; } = Price.Zero;
    public Price LineTotal { get; set; } = Price.Zero;
    public string? SpecificationsJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


