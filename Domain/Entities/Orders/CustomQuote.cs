namespace Domain.Orders;

public sealed partial class CustomQuote : TenantEntity
{
    public Guid OrderId { get; set; }
    public int Version { get; set; }
    public string? ReviewNotes { get; set; }
    public Price TotalAmount { get; set; } = Price.Zero;
    public Price DepositAmount { get; set; } = Price.Zero;
    public string CreatedByUserId { get; set; } = string.Empty;
    public DateTimeOffset? SentAt { get; set; }
    public string? SentVia { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


