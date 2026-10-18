using Domain.Entities.Common;

namespace Domain.Tenants;

public sealed class TenantRole : Entity
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}
