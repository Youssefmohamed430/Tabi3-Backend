using Domain.Audit;
using Domain.Files;
using Domain.Identity;
using Domain.Orders;
using Domain.Payments;
using Domain.Pricing;
using Domain.Printing;
using Domain.Subscriptions;
using Domain.Tenants;
using Domain.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class OrderUserReferenceConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b) => b.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class InvoiceUserReferenceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b) => b.HasOne(x => x.IssuedByUser).WithMany().HasForeignKey(x => x.IssuedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class OrderStatusHistoryUserReferenceConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> b) => b.HasOne(x => x.ActorUser).WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class OrderPaymentProofUserReferenceConfiguration : IEntityTypeConfiguration<OrderPaymentProof>
{
    public void Configure(EntityTypeBuilder<OrderPaymentProof> b)
    {
        b.HasOne(x => x.SubmittedByUser).WithMany().HasForeignKey(x => x.SubmittedByUserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.ReviewedByUser).WithMany().HasForeignKey(x => x.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class StoredFileUserReferenceConfiguration : IEntityTypeConfiguration<StoredFile>
{
    public void Configure(EntityTypeBuilder<StoredFile> b) => b.HasOne(x => x.UploadedByUser).WithMany().HasForeignKey(x => x.UploadedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class PriceListUserReferenceConfiguration : IEntityTypeConfiguration<PriceListItem>
{
    public void Configure(EntityTypeBuilder<PriceListItem> b) => b.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class CustomQuoteUserReferenceConfiguration : IEntityTypeConfiguration<CustomQuote>
{
    public void Configure(EntityTypeBuilder<CustomQuote> b) => b.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class SubscriptionUserReferenceConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> b) => b.HasOne(x => x.CreatedByUser).WithMany().HasForeignKey(x => x.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class SaaSPaymentUserReferenceConfiguration : IEntityTypeConfiguration<SaaSSubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SaaSSubscriptionPayment> b) => b.HasOne(x => x.ReviewedByUser).WithMany().HasForeignKey(x => x.ReviewedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class PrintDeviceUserReferenceConfiguration : IEntityTypeConfiguration<PrintDevice>
{
    public void Configure(EntityTypeBuilder<PrintDevice> b) => b.HasOne(x => x.PairedByUser).WithMany().HasForeignKey(x => x.PairedByUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class WhatsAppConversationUserReferenceConfiguration : IEntityTypeConfiguration<WhatsAppConversation>
{
    public void Configure(EntityTypeBuilder<WhatsAppConversation> b) => b.HasOne(x => x.AssignedUser).WithMany().HasForeignKey(x => x.AssignedUserId).OnDelete(DeleteBehavior.Restrict);
}

public sealed class AuditLogUserReferenceConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b) => b.HasOne(x => x.ActorUser).WithMany().HasForeignKey(x => x.ActorUserId).OnDelete(DeleteBehavior.Restrict);
}
