namespace Domain.Printing;

public sealed partial class PrintDevice : TenantEntity
{
    public string DeviceName { get; set; } = string.Empty;
    public PrintDevicePlatform Platform { get; set; }
    public string? PairedByUserId { get; set; }
    public string SecretReference { get; set; } = string.Empty;
    public DateTimeOffset? LastSeenAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


