using Domain.Enums;
using Domain.Entities.Common;

namespace Domain.Orders;

public sealed partial class OrderStatusHistory : TenantEntity
{
    
    public Guid OrderId { get; set; }
    public OrderStatus? FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string? ActorUserId { get; set; }
    public string Source { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
}


