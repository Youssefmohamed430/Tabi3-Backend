using Domain.Entities.Common;
using Domain.Enums;

namespace Domain.WhatsApp;

public sealed partial class WhatsAppConnection : TenantEntity
{
    public string PhoneNumberId { get; set; } = string.Empty;
    public string WabaId { get; set; } = string.Empty;
    public string? DisplayPhone { get; set; }
    public string SecretReference { get; set; } = string.Empty;
    public WhatsAppConnectionStatus Status { get; set; }
    public DateTimeOffset? ConnectedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


