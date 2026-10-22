namespace Infrastructure.Persistence.Configurations;

public sealed class PriceListItemConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> b)
    { b.ToTable("price_list_items", t => t.HasCheckConstraint("ck_price_list_item_unit_price", "\"UnitPrice\" >= 0")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Code).HasMaxLength(64).IsRequired(); b.Property(x => x.NameAr).HasMaxLength(200).IsRequired(); b.Property(x => x.NameEn).HasMaxLength(200); b.Property(x => x.BillingUnit).HasMaxLength(64).IsRequired(); b.Property(x => x.UnitPrice).HasPrecision(12, 2); b.HasIndex(x => new { x.TenantId, x.Code }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.IsActive, x.EffectiveFrom }); }
}

