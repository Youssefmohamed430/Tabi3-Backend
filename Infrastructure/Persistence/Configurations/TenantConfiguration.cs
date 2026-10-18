using Domain.Tenants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    { b.ToTable("tenants"); b.HasKey(x => x.Id); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.Slug).HasMaxLength(100).IsRequired(); b.HasIndex(x => x.Slug).IsUnique(); b.Property(x => x.BusinessName).HasMaxLength(200); b.Property(x => x.LogoObjectKey).HasMaxLength(1024); b.Property(x => x.PrimaryColor).HasMaxLength(7); b.Property(x => x.SecondaryColor).HasMaxLength(7); b.Property(x => x.AccentColor).HasMaxLength(7); b.Property(x => x.BackgroundColor).HasMaxLength(7); b.Property(x => x.PublicDescription).HasMaxLength(2000); b.Property(x => x.Phone).HasMaxLength(32); b.Property(x => x.Email).HasMaxLength(254); b.Property(x => x.Address).HasMaxLength(500); b.Property(x => x.OwnerUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.ApprovedByUserId).HasMaxLength(450); b.Property(x => x.Currency).HasMaxLength(3).IsFixedLength().IsRequired(); b.Property(x => x.TimeZone).HasMaxLength(80).IsRequired(); b.HasIndex(x => x.Status); }
}

public sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> b)
    { b.ToTable("tenant_memberships"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.PermissionsJson).HasColumnType("jsonb"); b.HasIndex(x => new { x.TenantId, x.UserId }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.Status, x.Role }); }
}

public sealed class TenantBusinessHoursConfiguration : IEntityTypeConfiguration<TenantBusinessHours>
{
    public void Configure(EntityTypeBuilder<TenantBusinessHours> b)
    { b.ToTable("tenant_business_hours", t => t.HasCheckConstraint("ck_business_hours_day", "\"DayOfWeek\" >= 0 AND \"DayOfWeek\" <= 6")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.HasIndex(x => new { x.TenantId, x.DayOfWeek }).IsUnique(); }
}

public sealed class TenantClosureConfiguration : IEntityTypeConfiguration<TenantClosure>
{
    public void Configure(EntityTypeBuilder<TenantClosure> b)
    { b.ToTable("tenant_closures", t => t.HasCheckConstraint("ck_tenant_closures_range", "\"EndsAt\" > \"StartsAt\"")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Label).HasMaxLength(200).IsRequired(); b.HasIndex(x => new { x.TenantId, x.StartsAt, x.EndsAt }); }
}

