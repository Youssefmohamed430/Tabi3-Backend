namespace Domain.Subscriptions;

public sealed partial class SaaSSubscriptionPayment : TenantEntity
{
    public Guid SubscriptionId { get; set; }
    public Price Amount { get; set; } = Price.Zero;
    public string Currency { get; set; } = "EGP";
    public SaaSPaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public Guid? ProofFileId { get; set; }
    public SubscriptionPaymentStatus Status { get; set; }
    public string? ReviewedByUserId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


