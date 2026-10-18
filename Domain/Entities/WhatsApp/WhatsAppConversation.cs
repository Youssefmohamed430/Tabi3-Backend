using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.WhatsApp;

public sealed partial class WhatsAppConversation : TenantEntity
{
    public Guid? CustomerId { get; set; }
    public string PhoneNormalized { get; set; } = string.Empty;
    public ConversationState State { get; set; }
    public string? AssignedUserId { get; set; }
    public DateTimeOffset? LastInboundAt { get; set; }
    public Guid? LinkedOrderId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


