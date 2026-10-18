using Domain.Audit;
using Domain.Enums;
using Domain.Notifications;
using Domain.Orders;
using Domain.Printing;
using Domain.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CustomQuoteConfiguration : IEntityTypeConfiguration<CustomQuote>
{
    public void Configure(EntityTypeBuilder<CustomQuote> b)
    { b.ToTable("custom_quotes", t => t.HasCheckConstraint("ck_custom_quotes_version", "\"Version\" > 0")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.ReviewNotes).HasMaxLength(2000); b.Property(x => x.CreatedByUserId).HasMaxLength(450).IsRequired(); b.Property(x => x.SentVia).HasMaxLength(32); b.HasIndex(x => new { x.TenantId, x.OrderId, x.Version }).IsUnique(); b.HasOne(x => x.Order).WithMany().HasForeignKey(x => new { x.TenantId, x.OrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class WhatsAppConnectionConfiguration : IEntityTypeConfiguration<WhatsAppConnection>
{
    public void Configure(EntityTypeBuilder<WhatsAppConnection> b)
    { b.ToTable("whatsapp_connections"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.PhoneNumberId).HasMaxLength(128).IsRequired(); b.Property(x => x.WabaId).HasMaxLength(128).IsRequired(); b.Property(x => x.DisplayPhone).HasMaxLength(32); b.Property(x => x.SecretReference).HasMaxLength(512).IsRequired(); b.HasIndex(x => new { x.TenantId, x.PhoneNumberId }).IsUnique(); b.HasIndex(x => new { x.TenantId, x.Status }); }
}

public sealed class WhatsAppConversationConfiguration : IEntityTypeConfiguration<WhatsAppConversation>
{
    public void Configure(EntityTypeBuilder<WhatsAppConversation> b)
    { b.ToTable("whatsapp_conversations"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.PhoneNormalized).HasMaxLength(32).IsRequired(); b.Property(x => x.AssignedUserId).HasMaxLength(450); b.HasIndex(x => new { x.TenantId, x.PhoneNormalized, x.State }); b.HasIndex(x => new { x.TenantId, x.UpdatedAt }); b.HasOne(x => x.Customer).WithMany().HasForeignKey(x => new { x.TenantId, x.CustomerId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.LinkedOrder).WithMany().HasForeignKey(x => new { x.TenantId, x.LinkedOrderId }).HasPrincipalKey(x => new { x.TenantId, x.OrderId }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class WhatsAppMessageConfiguration : IEntityTypeConfiguration<WhatsAppMessage>
{
    public void Configure(EntityTypeBuilder<WhatsAppMessage> b)
    { b.ToTable("whatsapp_messages"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.ProviderMessageId).HasMaxLength(256); b.Property(x => x.BodyRedacted).HasMaxLength(4000); b.HasIndex(x => new { x.TenantId, x.ProviderMessageId }).IsUnique().HasFilter("\"ProviderMessageId\" IS NOT NULL"); b.HasIndex(x => new { x.TenantId, x.ConversationId, x.CreatedAt }); b.HasOne(x => x.Conversation).WithMany().HasForeignKey(x => new { x.TenantId, x.ConversationId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Cascade); b.HasOne(x => x.MediaFile).WithMany().HasForeignKey(x => new { x.TenantId, x.MediaFileId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class NotificationOutboxConfiguration : IEntityTypeConfiguration<NotificationOutbox>
{
    public void Configure(EntityTypeBuilder<NotificationOutbox> b)
    { b.ToTable("notification_outbox", t => t.HasCheckConstraint("ck_notification_attempt_count", "\"AttemptCount\" >= 0")); b.HasKey(x => x.Id); b.Property(x => x.EventKey).HasMaxLength(256).IsRequired(); b.Property(x => x.Recipient).HasMaxLength(320).IsRequired(); b.Property(x => x.PayloadJson).HasColumnType("jsonb").IsRequired(); b.Property(x => x.LastErrorCode).HasMaxLength(128); b.HasIndex(x => new { x.TenantId, x.EventKey, x.Channel }).IsUnique().HasFilter("\"TenantId\" IS NOT NULL"); b.HasIndex(x => new { x.EventKey, x.Channel }).IsUnique().HasFilter("\"TenantId\" IS NULL"); b.HasIndex(x => new { x.Status, x.NextAttemptAt }); b.HasOne(x => x.Tenant).WithMany().HasForeignKey(x => x.TenantId).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class PrintDeviceConfiguration : IEntityTypeConfiguration<PrintDevice>
{
    public void Configure(EntityTypeBuilder<PrintDevice> b)
    { b.ToTable("print_devices"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.DeviceName).HasMaxLength(160).IsRequired(); b.Property(x => x.PairedByUserId).HasMaxLength(450); b.Property(x => x.SecretReference).HasMaxLength(512).IsRequired(); b.HasIndex(x => new { x.TenantId, x.RevokedAt, x.LastSeenAt }); }
}

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    { b.ToTable("audit_logs"); b.HasKey(x => x.Id); b.Property(x => x.ActorUserId).HasMaxLength(450); b.Property(x => x.Action).HasMaxLength(128).IsRequired(); b.Property(x => x.EntityType).HasMaxLength(128).IsRequired(); b.Property(x => x.CorrelationId).HasMaxLength(128); b.Property(x => x.MetadataJson).HasColumnType("jsonb"); b.HasIndex(x => new { x.TenantId, x.CreatedAt }); b.HasIndex(x => new { x.ActorUserId, x.CreatedAt }); b.HasIndex(x => new { x.EntityType, x.EntityId, x.CreatedAt }); }
}
