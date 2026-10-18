using Domain.Enums;
using Domain.Entities.Common;
using Domain.ValueObjects;

namespace Domain.Orders;

public sealed partial class Invoice : TenantEntity
{
    
    public Guid OrderId { get; set; }
    public long InvoiceNumber { get; set; }
    public InvoiceStatus Status { get; set; }
    public Price Subtotal { get; set; } = Price.Zero;
    public Price DiscountAmount { get; set; } = Price.Zero;
    public Price TotalAmount { get; set; } = Price.Zero;
    public Price DepositAmount { get; set; } = Price.Zero;
    public DateTimeOffset? IssuedAt { get; set; }
    public string? IssuedByUserId { get; set; }
    public Guid? PdfFileId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


