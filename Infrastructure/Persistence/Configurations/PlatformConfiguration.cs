namespace Infrastructure.Persistence.Configurations;

public sealed class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> b)
    {
        b.HasData(new IdentityRole
        {
            Id = DomainConstants.IdentityRoles.SuperAdminId,
            Name = DomainConstants.IdentityRoles.SuperAdmin,
            NormalizedName = DomainConstants.IdentityRoles.SuperAdmin.ToUpperInvariant(),
            ConcurrencyStamp = DomainConstants.IdentityRoles.SuperAdminId
        });
    }
}

public sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> b)
    { b.ToTable("asp_net_users"); b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired(); b.Property(x => x.PreferredLanguage).HasMaxLength(8).IsRequired(); b.Property(x => x.PhoneNormalized).HasMaxLength(32); b.HasIndex(x => x.NormalizedEmail).IsUnique().HasFilter("\"NormalizedEmail\" IS NOT NULL"); b.HasIndex(x => x.PhoneNormalized).IsUnique().HasFilter("\"PhoneNormalized\" IS NOT NULL"); }
}

public sealed class TenantMembershipIdentityConfiguration : IEntityTypeConfiguration<TenantMembership>
{
    public void Configure(EntityTypeBuilder<TenantMembership> b)
    { b.HasOne(x => x.User).WithMany(x => x.Memberships).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.InvitedByUser).WithMany(x => x.InvitedMemberships).HasForeignKey(x => x.InvitedByUserId).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class TenantIdentityConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> b)
    { b.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.ApprovedByUser).WithMany().HasForeignKey(x => x.ApprovedByUserId).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class SaaSPlanConfiguration : IEntityTypeConfiguration<SaaSPlan>
{
    public void Configure(EntityTypeBuilder<SaaSPlan> b)
    { b.ToTable("plans", t => { t.HasCheckConstraint("ck_plans_monthly_price", "\"MonthlyPrice\" >= 0"); t.HasCheckConstraint("ck_plans_yearly_price", "\"YearlyPrice\" >= 0"); t.HasCheckConstraint("ck_plans_limits", "(\"MaxStaffUsers\" IS NULL OR \"MaxStaffUsers\" >= 0) AND (\"MaxOrdersPerMonth\" IS NULL OR \"MaxOrdersPerMonth\" >= 0) AND (\"MaxStorageBytes\" IS NULL OR \"MaxStorageBytes\" >= 0) AND (\"MaxPrintDevices\" IS NULL OR \"MaxPrintDevices\" >= 0) AND (\"MaxWhatsAppNumbers\" IS NULL OR \"MaxWhatsAppNumbers\" >= 0)"); }); b.HasKey(x => x.Id); b.Property(x => x.Code).HasMaxLength(64).IsRequired(); b.Property(x => x.NameAr).HasMaxLength(120).IsRequired(); b.Property(x => x.NameEn).HasMaxLength(120).IsRequired(); b.HasIndex(x => x.Code).IsUnique(); b.HasIndex(x => new { x.IsActive, x.Code }); }
}

public sealed class TenantSubscriptionConfiguration : IEntityTypeConfiguration<TenantSubscription>
{
    public void Configure(EntityTypeBuilder<TenantSubscription> b)
    { b.ToTable("tenant_subscriptions"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.HasIndex(x => new { x.TenantId, x.Status, x.EndsAt }); b.HasOne(x => x.Plan).WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class SaaSSubscriptionPaymentConfiguration : IEntityTypeConfiguration<SaaSSubscriptionPayment>
{
    public void Configure(EntityTypeBuilder<SaaSSubscriptionPayment> b)
    { b.ToTable("saas_subscription_payments", t => t.HasCheckConstraint("ck_saas_subscription_payments_amount", "\"Amount\" >= 0")); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Currency).HasMaxLength(3).IsFixedLength(); b.Property(x => x.Reference).HasMaxLength(128); b.HasIndex(x => new { x.TenantId, x.SubscriptionId, x.Status, x.CreatedAt }); b.HasOne(x => x.Subscription).WithMany().HasForeignKey(x => new { x.TenantId, x.SubscriptionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.ProofFile).WithMany().HasForeignKey(x => new { x.TenantId, x.ProofFileId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    { b.ToTable("refresh_tokens"); b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired(); b.Property(x => x.ReplacedByTokenHash).HasMaxLength(128); b.Property(x => x.CreatedByIp).HasMaxLength(64); b.HasIndex(x => x.TokenHash).IsUnique(); b.HasIndex(x => new { x.UserId, x.ExpiresAt, x.RevokedAt }); b.HasOne(x => x.User).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade); }
}

public sealed class AccountTokenConfiguration : IEntityTypeConfiguration<AccountToken>
{
    public void Configure(EntityTypeBuilder<AccountToken> b)
    { b.ToTable("account_tokens"); b.HasKey(x => x.Id); b.Property(x => x.UserId).HasMaxLength(450).IsRequired(); b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired(); b.Property(x => x.MetadataJson).HasColumnType("jsonb"); b.HasIndex(x => x.TokenHash).IsUnique(); b.HasIndex(x => new { x.UserId, x.Purpose, x.ExpiresAt }); b.HasOne(x => x.User).WithMany(x => x.AccountTokens).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade); b.HasOne(x => x.Membership).WithMany().HasForeignKey(x => new { x.TenantId, x.MembershipId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class TenantRegistrationRequestConfiguration : IEntityTypeConfiguration<TenantRegistrationRequest>
{
    public void Configure(EntityTypeBuilder<TenantRegistrationRequest> b)
    { b.ToTable("tenant_registration_requests"); b.HasKey(x => x.Id); b.Property(x => x.BusinessName).HasMaxLength(200).IsRequired(); b.Property(x => x.Slug).HasMaxLength(100).IsRequired(); b.Property(x => x.Phone).HasMaxLength(32).IsRequired(); b.Property(x => x.Email).HasMaxLength(254); b.Property(x => x.ActivationMode).HasMaxLength(24); b.Property(x => x.DecidedByUserId).HasMaxLength(450); b.HasIndex(x => x.Slug).IsUnique(); b.HasIndex(x => new { x.Status, x.CreatedAt }); b.HasOne(x => x.OwnerUser).WithMany().HasForeignKey(x => x.OwnerUserId).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.DecidedByUser).WithMany().HasForeignKey(x => x.DecidedByUserId).OnDelete(DeleteBehavior.Restrict); }
}

public sealed class TenantDomainConfiguration : IEntityTypeConfiguration<TenantDomain>
{
    public void Configure(EntityTypeBuilder<TenantDomain> b)
    { b.ToTable("tenant_domains"); b.HasKey(x => x.Id); b.HasAlternateKey(x => new { x.TenantId, x.Id }); b.Property(x => x.Hostname).HasMaxLength(253).IsRequired(); b.HasIndex(x => x.Hostname).IsUnique(); b.HasIndex(x => new { x.TenantId, x.Status, x.IsPrimary }); }
}

