using Domain.Entities.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Subscriptions;

public sealed partial class TenantSubscription : TenantEntity
{
    public Guid PlanId { get; set; }
    public SubscriptionStatus Status { get; set; }
    public BillingPeriod BillingPeriod { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset? EndsAt { get; set; }
    public DateTimeOffset? GraceEndsAt { get; set; }
    public Price PriceSnapshot { get; set; } = Price.Zero;
    public string? CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


