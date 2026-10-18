using Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Identity;

namespace Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    { b.ToTable("customers"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Name).HasMaxLength(200).IsRequired(); b.Property(x => x.Phone).HasMaxLength(32).IsRequired(); b.Property(x => x.NormalizedPhone).HasMaxLength(32).IsRequired(); b.Property(x => x.Email).HasMaxLength(254); b.HasIndex(x => new { x.TenantId, x.NormalizedPhone }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.IsActive, x.Name }); b.HasIndex(x => new { x.TenantId, x.PortalUserId }).IsUnique().HasFilter("\"PortalUserId\" IS NOT NULL"); }
}

public sealed class CustomerIdentityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    { b.HasOne(x => x.PortalUser).WithMany().HasForeignKey(x => x.PortalUserId).OnDelete(DeleteBehavior.Restrict); }
}

