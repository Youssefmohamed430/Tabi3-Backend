namespace Domain.Notifications;

public sealed partial class UserNotification : Entity
{
    public string UserId { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string EventKey { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? ReadAt { get; private set; }

    public void MarkAsRead(DateTimeOffset readAt)
    {
        ReadAt ??= readAt;
    }
}
