namespace Infrastructure.Identity;

public sealed class JwtOptions
{
    public const string SectionName = "JWT";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int DurationInSeconds { get; set; } = 900;
}
