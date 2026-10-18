using Microsoft.EntityFrameworkCore;
using Domain.Common;
using System.Linq.Expressions;

namespace Infrastructure.Persistence;

public sealed class Tabi3DbContext(DbContextOptions<Tabi3DbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Tabi3DbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateNotDeletedFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression CreateNotDeletedFilter(Type entityType)
    {
        var entity = Expression.Parameter(entityType, "entity");
        var isDeleted = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [typeof(bool)],
            entity,
            Expression.Constant(nameof(ISoftDeletable.IsDeleted)));

        return Expression.Lambda(Expression.Not(isDeleted), entity);
    }
}
