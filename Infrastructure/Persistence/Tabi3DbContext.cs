using Microsoft.EntityFrameworkCore;
using Domain.Entities.Common;
using Domain.Tenants;
using Domain.ValueObjects;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence;

public sealed class Tabi3DbContext(DbContextOptions<Tabi3DbContext> options) : IdentityDbContext<Tabi3User, IdentityRole, string>(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<DateTimeOffset>().HaveConversion<EgyptianDateTimeConverter>();
        configurationBuilder.Properties<DateTimeOffset?>().HaveConversion<NullableEgyptianDateTimeConverter>();
        configurationBuilder.Properties<Price>().HaveConversion<PriceConverter>().HavePrecision(12, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Tabi3DbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(CreateNotDeletedFilter(entityType.ClrType));
                if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
                    modelBuilder.Entity(entityType.ClrType)
                        .HasOne(typeof(Tenant), nameof(TenantEntity.Tenant))
                        .WithMany()
                        .HasForeignKey(nameof(TenantEntity.TenantId))
                        .OnDelete(DeleteBehavior.Restrict);
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
