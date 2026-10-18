using Domain.Enums;
using Domain.Entities.Common;

namespace Domain.Tenants;

public sealed partial class TenantClosure : TenantEntity
{
    
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public string Label { get; set; } = string.Empty;
}


