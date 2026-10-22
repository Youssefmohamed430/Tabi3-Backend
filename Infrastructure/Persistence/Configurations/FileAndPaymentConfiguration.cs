namespace Infrastructure.Persistence.Configurations;

public sealed class StoredFileConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> b)
    { b.ToTable("stored_files", t => t.HasCheckConstraint("ck_stored_files_size", "\"SizeBytes\" >= 0")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.ObjectKey).HasMaxLength(1024).IsRequired(); b.Property(x => x.OriginalName).HasMaxLength(255).IsRequired(); b.Property(x => x.ContentType).HasMaxLength(255).IsRequired(); b.HasIndex(x => new { x.TenantId, x.ObjectKey }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.OrderId }); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class OrderPaymentProofConfiguration : IEntityTypeConfiguration<OrderPaymentProof>
{
    public void Configure(EntityTypeBuilder<OrderPaymentProof> b)
    { b.ToTable("order_payment_proofs", t => t.HasCheckConstraint("ck_payment_proofs_amounts", "(\"ClaimedAmount\" IS NULL OR \"ClaimedAmount\" >= 0) AND (\"ConfirmedAmount\" IS NULL OR \"ConfirmedAmount\" >= 0)")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.ClaimedAmount).HasPrecision(12, 2); b.Property(x => x.ConfirmedAmount).HasPrecision(12, 2); b.Property(x => x.Reference).HasMaxLength(128); b.Property(x => x.RejectionReason).HasMaxLength(1000); b.HasIndex(x => new { x.TenantId, x.OrderId, x.Status, x.SubmittedAt }); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.File).WithMany().HasForeignKey(x => new { x.TenantId, x.FileId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}


