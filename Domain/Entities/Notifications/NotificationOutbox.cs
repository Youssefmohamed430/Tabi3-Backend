namespace Domain.Notifications;

public sealed partial class NotificationOutbox : Entity
{
    public Guid? TenantId { get; set; }
    public string EventKey { get; set; } = string.Empty;
    public NotificationChannel Channel { get; set; }
    public string Recipient { get; set; } = string.Empty;
    public string PayloadJson { get; set; } = "{}";
    public NotificationStatus Status { get; set; }
    public int AttemptCount { get; set; }
    public DateTimeOffset? NextAttemptAt { get; set; }
    public string? LastErrorCode { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? SentAt { get; set; }
}


