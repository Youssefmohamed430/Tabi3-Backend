namespace Domain.Identity;

public sealed partial class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public string? PhoneNormalized { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? LastLoginAt { get; set; }
}

