namespace Domain.Tenants;

public sealed partial class TenantDomain : TenantEntity
{
    public string Hostname { get; set; } = string.Empty;
    public TenantDomainType Type { get; set; }
    public TenantDomainStatus Status { get; set; }
    public bool IsPrimary { get; set; }
    public DateTimeOffset? VerifiedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset UpdatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


