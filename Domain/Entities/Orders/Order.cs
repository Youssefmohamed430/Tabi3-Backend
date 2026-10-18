using Domain.Enums;
using Domain.Entities.Common;
using Domain.ValueObjects;
using System.Security.Cryptography;

namespace Domain.Orders;


public sealed partial class Order : TenantEntity
{
    public Guid OrderId { get => Id; set => Id = value; }
    public static long GenerateOrderNumber() => RandomNumberGenerator.GetInt32(10_000_000, 100_000_000);
    public long OrderNumber { get; set; } = GenerateOrderNumber();
    public Guid CustomerId { get; set; }
    public OrderSource Source { get; set; }
    public OrderStatus Status { get; set; }
    public OrderPriority Priority { get; set; }
    public PricingType PricingType { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public string Currency { get; set; } = Domain.Constants.DomainConstants.DefaultCurrency;
    public Price Subtotal { get; set; } = Price.Zero;
    public Price DiscountAmount { get; set; } = Price.Zero;
    public Price TotalAmount { get; set; } = Price.Zero;
    public Price DepositRequired { get; set; } = Price.Zero;
    public Price AmountPaid { get; set; } = Price.Zero;
    public Price BalanceDue { get; set; } = Price.Zero;
    public DateTimeOffset? RequestedDueAt { get; set; }
    public DateTimeOffset? ProductionEligibleAt { get; set; }
    public int? EstimatedWorkMinutes { get; set; }
    public DateTimeOffset? EstimatedReadyAt { get; set; }
    public DateTimeOffset? ReadyEstimateCalculatedAt { get; set; }
    public DateTimeOffset? ReadyAt { get; set; }
    public string? NotesInternal { get; set; }
    public string? NotesCustomer { get; set; }
    public string? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? CompletedAt { get; set; }
    public long RowVersion { get; set; }
}


