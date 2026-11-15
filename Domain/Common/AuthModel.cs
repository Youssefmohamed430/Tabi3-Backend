namespace Domain.Common;

public class AuthModel
{
    public string? Id { get; set; }
    public string? Message { get; set; }
    public bool IsAuthenticated { get; set; } = false;
    public string? Username { get; set; }
    public string? Email { get; set; }
    public List<string>? Roles { get; set; }
    public List<TenantLoginOption>? AvailableTenants { get; set; }
    public Guid? TenantId { get; set; }
    public string? TenantSlug { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresOn { get; set; }
    [JsonIgnore]
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}

public sealed record TenantLoginOption(Guid TenantId, string TenantName, string TenantSlug, string RoleCode);
