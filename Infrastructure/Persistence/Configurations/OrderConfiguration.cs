namespace Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders", t => { t.HasCheckConstraint("ck_orders_amounts", "\"Subtotal\" >= 0 AND \"DiscountAmount\" >= 0 AND \"TotalAmount\" >= 0 AND \"DepositRequired\" >= 0 AND \"AmountPaid\" >= 0 AND \"BalanceDue\" >= 0"); t.HasCheckConstraint("ck_orders_balance", "\"BalanceDue\" = \"TotalAmount\" - \"AmountPaid\""); t.HasCheckConstraint("ck_orders_estimated_work_minutes", "\"EstimatedWorkMinutes\" IS NULL OR \"EstimatedWorkMinutes\" >= 0"); });
        b.Ignore(x => x.Id); b.HasKey(x => x.OrderId); b.HasAlternateKey(x => new { x.TenantId, x.OrderId }); b.Property(x => x.OrderId).ValueGeneratedNever(); b.Property(x => x.Currency).HasMaxLength(3).IsFixedLength().IsRequired(); foreach (var p in new[] { "Subtotal", "DiscountAmount", "TotalAmount", "DepositRequired", "AmountPaid", "BalanceDue" }) b.Property<Price>(p).HasPrecision(12, 2);
        b.HasIndex(x => new { x.TenantId, x.OrderNumber }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.Status, x.CreatedAt }); b.HasIndex(x => new { x.TenantId, x.CustomerId, x.CreatedAt }); b.HasIndex(x => new { x.TenantId, x.RequestedDueAt }); b.HasIndex(x => new { x.TenantId, x.EstimatedReadyAt });
        b.HasOne(x => x.Customer).WithMany().HasForeignKey(x => new { x.TenantId, x.CustomerId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    { b.ToTable("order_items", t => { t.HasCheckConstraint("ck_order_items_counts", "\"PageCount\" >= 0 AND \"CopyCount\" > 0 AND \"BillableUnits\" >= 0"); t.HasCheckConstraint("ck_order_items_amounts", "\"UnitPrice\" >= 0 AND \"LineTotal\" >= 0"); }); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Description).HasMaxLength(500).IsRequired(); b.Property(x => x.SpecificationsJson).HasColumnType("jsonb"); b.HasIndex(x => new { x.TenantId, x.OrderId }); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Cascade); b.HasOne(x => x.PriceListItem).WithMany().HasForeignKey(x => new { x.TenantId, x.PriceListItemId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    { b.ToTable("invoices", t => t.HasCheckConstraint("ck_invoices_amounts", "\"Subtotal\" >= 0 AND \"DiscountAmount\" >= 0 AND \"TotalAmount\" >= 0 AND \"DepositAmount\" >= 0")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); foreach (var p in new[] { "Subtotal", "DiscountAmount", "TotalAmount", "DepositAmount" }) b.Property<Price>(p).HasPrecision(12, 2); b.HasIndex(x => new { x.TenantId, x.InvoiceNumber }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.OrderId, x.CreatedAt }); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.PdfFile).WithMany().HasForeignKey(x => new { x.TenantId, x.PdfFileId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> b)
    { b.ToTable("order_status_history"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Source).HasMaxLength(32).IsRequired(); b.Property(x => x.Reason).HasMaxLength(1000); b.HasIndex(x => new { x.TenantId, x.OrderId, x.CreatedAt }); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Cascade); }
}





