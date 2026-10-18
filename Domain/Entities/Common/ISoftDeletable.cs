using Domain.Entities.Common;
using Domain.Enums;
namespace Domain.Entities.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}


