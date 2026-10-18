using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Application.Abstractions.Persistence;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class EfRepository<TEntity>(Tabi3DbContext dbContext) : IRepository<TEntity>
    where TEntity : class, ISoftDeletable
{
    private readonly DbSet<TEntity> _entities = dbContext.Set<TEntity>();

    public Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        await ApplySpecification(specification).ToListAsync(cancellationToken);

    public Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default) =>
        ApplySpecification(specification).CountAsync(cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) =>
        await _entities.AddAsync(entity, cancellationToken);

    public void Update(TEntity entity) => _entities.Update(entity);

    public void Remove(TEntity entity)
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Added)
        {
            entry.State = EntityState.Detached;
            return;
        }

        entity.IsDeleted = true;
        entry.Property(nameof(ISoftDeletable.IsDeleted)).IsModified = true;
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification) =>
        SpecificationEvaluator.Default.GetQuery(_entities.AsQueryable(), specification);
}
