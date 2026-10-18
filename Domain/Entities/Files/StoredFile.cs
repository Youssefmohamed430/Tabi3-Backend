using Domain.Enums;
using Domain.Entities.Common;

namespace Domain.Files;

public sealed partial class StoredFile : TenantEntity
{
    
    public string ObjectKey { get; set; } = string.Empty;
    public string OriginalName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string? Sha256 { get; set; }
    public StoredFileCategory Category { get; set; }
    public string? UploadedByUserId { get; set; }
    public Guid? OrderId { get; set; }
    public FileScanStatus ScanStatus { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? RetentionUntil { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}


