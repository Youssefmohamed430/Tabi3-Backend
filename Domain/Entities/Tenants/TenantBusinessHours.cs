using Domain.Enums;
using Domain.Entities.Common;

namespace Domain.Tenants;

public sealed partial class TenantBusinessHours : TenantEntity
{
    
    public short DayOfWeek { get; set; }
    public TimeOnly? OpensAt { get; set; }
    public TimeOnly? ClosesAt { get; set; }
    public bool IsClosed { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


