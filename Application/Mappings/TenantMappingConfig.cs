#nullable enable

namespace Application.Mappings;

public sealed class TenantMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Tenant, Tenant>()
            .Map(target => target.Name, source => source.Name.Trim())
            .Map(target => target.BusinessName, source => NormalizeOptional(source.BusinessName, 200))
            .Map(target => target.PublicDescription, source => NormalizeOptional(source.PublicDescription, 2000))
            .Map(target => target.Phone, source => NormalizeOptional(source.Phone, 32))
            .Map(target => target.Email, source => NormalizeOptional(source.Email, 254))
            .Map(target => target.Address, source => NormalizeOptional(source.Address, 500))
            .Map(target => target.LogoObjectKey, source => NormalizeOptional(source.LogoObjectKey, 1024))
            .Map(target => target.PrimaryColor, source => NormalizeColor(source.PrimaryColor, "#142D4E"))
            .Map(target => target.SecondaryColor, source => NormalizeColor(source.SecondaryColor, "#17A6A1"))
            .Map(target => target.AccentColor, source => NormalizeColor(source.AccentColor, "#F2A541"))
            .Map(target => target.BackgroundColor, source => NormalizeColor(source.BackgroundColor, "#F7F5F0"))
            .Ignore(target => target.Id)
            .Ignore(target => target.Slug)
            .Ignore(target => target.OwnerUserId)
            .Ignore(target => target.Currency)
            .Ignore(target => target.TimeZone)
            .Ignore(target => target.Status)
            .Ignore(target => target.ApprovedAt)
            .Ignore(target => target.ApprovedByUserId)
            .Ignore(target => target.TrialStartedAt)
            .Ignore(target => target.TrialEndsAt)
            .Ignore(target => target.CreatedAt)
            .Ignore(target => target.UpdatedAt)
            .Ignore(target => target.IsDeleted);

        config.NewConfig<TenantRegistrationRequest, Tenant>()
            .Map(target => target.Name, source => source.BusinessName)
            .Map(target => target.BusinessName, source => source.BusinessName)
            .Map(target => target.Slug, source => source.Slug)
            .Map(target => target.Phone, source => source.Phone)
            .Map(target => target.Email, source => source.Email)
            .Map(target => target.OwnerUserId, source => source.OwnerUserId)
            .Map(target => target.Status, _ => TenantStatus.Active)
            .Ignore(target => target.Id)
            .Ignore(target => target.Status)
            .Ignore(target => target.ApprovedAt)
            .Ignore(target => target.ApprovedByUserId)
            .Ignore(target => target.TrialStartedAt)
            .Ignore(target => target.TrialEndsAt)
            .Ignore(target => target.CreatedAt)
            .Ignore(target => target.UpdatedAt)
            .Ignore(target => target.IsDeleted);
    }

    private static string? NormalizeOptional(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var normalized = value.Trim();
        return normalized.Length <= maxLength ? normalized : null;
    }

    private static string NormalizeColor(string? value, string fallback) =>
        !string.IsNullOrWhiteSpace(value) && System.Text.RegularExpressions.Regex.IsMatch(value, "^#[0-9A-Fa-f]{6}$")
            ? value.ToUpperInvariant()
            : fallback;
}
