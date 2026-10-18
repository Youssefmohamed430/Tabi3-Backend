using Domain.Audit;
using Domain.Customers;
using Domain.Files;
using Domain.Identity;
using Domain.Notifications;
using Domain.Orders;
using Domain.Payments;
using Domain.Pricing;
using Domain.Printing;
using Domain.Subscriptions;
using Domain.Tenants;
using Domain.WhatsApp;

namespace Domain.Entities.Common
{
    public abstract partial class TenantEntity
    {
        public Tenant? Tenant { get; set; }
    }
}

namespace Domain.Tenants
{
    public sealed partial class Tenant
    {
        public Tabi3User OwnerUser { get; set; } = null!;
        public Tabi3User? ApprovedByUser { get; set; }
    }

    public sealed partial class TenantMembership
    {
        public Tabi3User User { get; set; } = null!;
        public Tabi3User? InvitedByUser { get; set; }
    }

    public sealed partial class TenantRegistrationRequest
    {
        public Tabi3User OwnerUser { get; set; } = null!;
        public Tabi3User? DecidedByUser { get; set; }
    }
}

namespace Domain.Customers
{
    public sealed partial class Customer
    {
        public Tabi3User? PortalUser { get; set; }
    }
}

namespace Domain.Pricing
{
    public sealed partial class PriceListItem
    {
        public Tabi3User? CreatedByUser { get; set; }
    }
}

namespace Domain.Orders
{
    public sealed partial class Order
    {
        public Customer Customer { get; set; } = null!;
        public Tabi3User? CreatedByUser { get; set; }
    }

    public sealed partial class OrderItem
    {
        public Order Order { get; set; } = null!;
        public PriceListItem? PriceListItem { get; set; }
    }

    public sealed partial class Invoice
    {
        public Order Order { get; set; } = null!;
        public Tabi3User? IssuedByUser { get; set; }
        public StoredFile? PdfFile { get; set; }
    }

    public sealed partial class CustomQuote
    {
        public Order Order { get; set; } = null!;
        public Tabi3User CreatedByUser { get; set; } = null!;
    }

    public sealed partial class OrderStatusHistory
    {
        public Order Order { get; set; } = null!;
        public Tabi3User? ActorUser { get; set; }
    }
}

namespace Domain.Files
{
    public sealed partial class StoredFile
    {
        public Order? Order { get; set; }
        public Tabi3User? UploadedByUser { get; set; }
    }
}

namespace Domain.Payments
{
    public sealed partial class OrderPaymentProof
    {
        public Order Order { get; set; } = null!;
        public StoredFile File { get; set; } = null!;
        public Tabi3User? SubmittedByUser { get; set; }
        public Tabi3User? ReviewedByUser { get; set; }
    }
}

namespace Domain.Subscriptions
{
    public sealed partial class TenantSubscription
    {
        public SaaSPlan Plan { get; set; } = null!;
        public Tabi3User? CreatedByUser { get; set; }
    }

    public sealed partial class SaaSSubscriptionPayment
    {
        public TenantSubscription Subscription { get; set; } = null!;
        public StoredFile? ProofFile { get; set; }
        public Tabi3User? ReviewedByUser { get; set; }
    }
}

namespace Domain.Identity
{
    public sealed partial class RefreshToken
    {
        public Tabi3User User { get; set; } = null!;
    }

    public sealed partial class AccountToken
    {
        public Tabi3User User { get; set; } = null!;
        public TenantMembership? Membership { get; set; }
    }

    public sealed partial class Tabi3User
    {
        public ICollection<TenantMembership> Memberships { get; set; } = [];
        public ICollection<TenantMembership> InvitedMemberships { get; set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
        public ICollection<AccountToken> AccountTokens { get; set; } = [];
    }
}

namespace Domain.WhatsApp
{
    public sealed partial class WhatsAppConversation
    {
        public Customer? Customer { get; set; }
        public Order? LinkedOrder { get; set; }
        public Tabi3User? AssignedUser { get; set; }
    }

    public sealed partial class WhatsAppMessage
    {
        public WhatsAppConversation Conversation { get; set; } = null!;
        public StoredFile? MediaFile { get; set; }
    }
}

namespace Domain.Printing
{
    public sealed partial class PrintDevice
    {
        public Tabi3User? PairedByUser { get; set; }
    }
}

namespace Domain.Notifications
{
    public sealed partial class NotificationOutbox
    {
        public Tenant? Tenant { get; set; }
    }
}

namespace Domain.Audit
{
    public sealed partial class AuditLog
    {
        public Tenant? Tenant { get; set; }
        public Tabi3User? ActorUser { get; set; }
    }
}
