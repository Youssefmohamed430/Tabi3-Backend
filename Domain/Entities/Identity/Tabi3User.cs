using Microsoft.AspNetCore.Identity;

namespace Domain.Identity;

public sealed partial class Tabi3User : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "ar";
    public string? PhoneNormalized { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = Domain.Common.EgyptianDateTime.Now;
    public DateTimeOffset? LastLoginAt { get; set; }
}

