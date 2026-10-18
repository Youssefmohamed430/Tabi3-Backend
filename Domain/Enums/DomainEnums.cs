namespace Domain.Enums;

public enum TenantStatus { PendingActivation, Active, Suspended, Expired }
public enum MembershipStatus { PendingSetup, Active, Inactive }
public enum OrderSource { Staff, Website, WhatsApp, CustomerPortal }
public enum OrderStatus { Draft, AwaitingInformation, Pricing, CustomReview, AwaitingDeposit, ProofSubmitted, PaymentReview, PaymentRejected, Paid, InProduction, ReadyForPickup, Completed, Cancelled }
public enum OrderPriority { Normal, Urgent }
public enum PricingType { Standard, Custom }
public enum PaymentStatus { Unpaid, ProofSubmitted, PaymentReview, Paid, PartiallyPaid, Refunded }
public enum InvoiceStatus { Draft, Issued, Cancelled }
public enum StoredFileCategory { Print, PaymentProof, Invoice, Other }
public enum FileScanStatus { Pending, Clean, Rejected, Failed }
public enum PaymentProofStatus { Submitted, Approved, Rejected }
public enum SubscriptionStatus { TrialPending, TrialActive, PendingPayment, ActivePaid, Grace, Expired, Suspended }
public enum BillingPeriod { Monthly, Yearly }
public enum SubscriptionPaymentStatus { Pending, Approved, Rejected }
public enum TenantRegistrationStatus { Submitted, PendingReview, Approved, Rejected }
public enum TenantDomainType { PlatformSubdomain, Custom }
public enum TenantDomainStatus { PendingVerification, Active, Rejected }
public enum AccountTokenPurpose { AccountSetup, PasswordReset }
public enum SaaSPaymentMethod { BankTransfer, InstaPay, Wallet, Other }
public enum WhatsAppConnectionStatus { Pending, Connected, Disconnected, Failed }
public enum ConversationState { CollectingDetails, AwaitingFile, PendingStaff, LinkedToOrder, Closed }
public enum MessageDirection { Inbound, Outbound }
public enum WhatsAppMessageType { Text, Image, Document, Audio, Video, Interactive, Unknown }
public enum MessageDeliveryStatus { Received, Queued, Sent, Delivered, Read, Failed }
public enum NotificationChannel { WhatsApp, Email, InApp }
public enum NotificationStatus { Pending, Processing, Sent, Failed, DeadLetter }
public enum PrintDevicePlatform { Windows, MacOS, Linux }
