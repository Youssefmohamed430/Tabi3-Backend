namespace Domain.WhatsApp;

public sealed partial class WhatsAppMessage : TenantEntity
{
    public Guid ConversationId { get; set; }
    public string? ProviderMessageId { get; set; }
    public MessageDirection Direction { get; set; }
    public WhatsAppMessageType MessageType { get; set; }
    public string? BodyRedacted { get; set; }
    public Guid? MediaFileId { get; set; }
    public MessageDeliveryStatus DeliveryStatus { get; set; }
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


