namespace Infrastructure.Persistence;

public sealed class Tabi3DbContext(DbContextOptions<Tabi3DbContext> options, ICurrentTenant currentTenant) : IdentityDbContext<ApplicationUser, IdentityRole, string>(options)
{
    public Guid? CurrentTenantId => currentTenant.TenantId;
    public bool IsPlatformAdministrator => currentTenant.IsPlatformAdministrator;

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
            var entityBuilder = modelBuilder.Entity(entityType.ClrType);
            var isTenantEntity = typeof(TenantEntity).IsAssignableFrom(entityType.ClrType);
            var tenantIdProperty = entityType.FindProperty(nameof(TenantEntity.TenantId));
            var isSoftDeletable = typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType);

            if (tenantIdProperty is not null)
            {
                entityBuilder.HasQueryFilter(CreateTenantFilter(entityType.ClrType, tenantIdProperty.ClrType, isSoftDeletable));
            }
            else if (isSoftDeletable)
            {
                entityBuilder.HasQueryFilter(CreateNotDeletedFilter(entityType.ClrType));
            }

            if (isTenantEntity)
            {
                entityBuilder
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

    private LambdaExpression CreateTenantFilter(Type entityType, Type tenantIdPropertyType, bool isSoftDeletable)
    {
        var entity = Expression.Parameter(entityType, "entity");
        Expression tenantId = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            [tenantIdPropertyType],
            entity,
            Expression.Constant(nameof(TenantEntity.TenantId)));
        if (tenantId.Type == typeof(Guid))
            tenantId = Expression.Convert(tenantId, typeof(Guid?));

        var currentTenantId = Expression.Property(Expression.Constant(this), nameof(CurrentTenantId));
        var hasTenant = Expression.Property(currentTenantId, nameof(Nullable<Guid>.HasValue));
        var tenantMatches = Expression.Equal(tenantId, currentTenantId);
        var isPlatformAdministrator = Expression.Property(Expression.Constant(this), nameof(IsPlatformAdministrator));
        var tenantIsAllowed = Expression.OrElse(
            isPlatformAdministrator,
            Expression.AndAlso(hasTenant, tenantMatches));

        Expression filter = tenantIsAllowed;
        if (isSoftDeletable)
        {
            var isDeleted = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [typeof(bool)],
                entity,
                Expression.Constant(nameof(ISoftDeletable.IsDeleted)));
            filter = Expression.AndAlso(Expression.Not(isDeleted), filter);
        }

        return Expression.Lambda(filter, entity);
    }
}
