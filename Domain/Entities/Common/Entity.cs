using Domain.Entities.Common;
using Domain.Enums;
namespace Domain.Entities.Common;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public abstract partial class TenantEntity : Entity, ISoftDeletable
{
    public Guid TenantId { get; set; }
    public bool IsDeleted { get; set; }
    
    
}


