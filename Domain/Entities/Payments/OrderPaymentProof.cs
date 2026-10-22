namespace Domain.Payments;

public sealed partial class OrderPaymentProof : TenantEntity
{
    
    public Guid OrderId { get; set; }
    public Guid FileId { get; set; }
    public Price? ClaimedAmount { get; set; }
    public string? Reference { get; set; }
    public PaymentProofStatus Status { get; set; }
    public string? SubmittedByUserId { get; set; }
    public DateTimeOffset SubmittedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public string? ReviewedByUserId { get; set; }
    public DateTimeOffset? ReviewedAt { get; set; }
    public string? RejectionReason { get; set; }
    public Price? ConfirmedAmount { get; set; }
}


