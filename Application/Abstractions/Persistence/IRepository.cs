using Ardalis.Specification;
using Domain.Common;

namespace Application.Abstractions.Persistence;

public interface IRepository<TEntity> where TEntity : class, ISoftDeletable
{
    Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Remove(TEntity entity);
}
