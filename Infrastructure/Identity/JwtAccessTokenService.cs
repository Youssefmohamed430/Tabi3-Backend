namespace Infrastructure.Identity;

public sealed class JwtAccessTokenService(
    UserManager<ApplicationUser> userManager,
    Tabi3DbContext dbContext,
    IOptions<JwtOptions> options,
    IConfiguration configuration) : IAccessTokenService
{
    private readonly JwtOptions _options = options.Value;
    private readonly string _signingKey = configuration["JWTKey"]
        ?? throw new InvalidOperationException("JWTKey must be configured in the environment or .env file.");

    public async Task<AccessTokenResult> CreateAsync(string userId, Guid? tenantId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("The account is not available.");
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
        };

        if (!string.IsNullOrWhiteSpace(user.Email))
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

        var applicationRoles = await userManager.GetRolesAsync(user);

        if (tenantId is null)
        {
            if (!applicationRoles.Contains("SuperAdmin", StringComparer.Ordinal))
                throw new UnauthorizedAccessException("A tenant context is required for this account.");

            claims.AddRange(applicationRoles.Select(role => new Claim("role", role)));
        }
        else
        {
            var membership = await dbContext.Set<TenantMembership>()
                .AsNoTracking()
                .Include(item => item.Role)
                .Include(item => item.Tenant)
                .SingleOrDefaultAsync(item => item.TenantId == tenantId && item.UserId == user.Id, cancellationToken);

            if (membership is { Status: MembershipStatus.Active, Tenant.Status: TenantStatus.Active })
            {
                claims.Add(new Claim("tenant_id", membership.TenantId.ToString()));
                claims.Add(new Claim("membership_id", membership.Id.ToString()));
                claims.Add(new Claim("tenant_role", membership.Role.Code));
                claims.Add(new Claim("role", membership.Role.Code));
            }
            else
            {
                var customer = await dbContext.Set<Customer>()
                    .AsNoTracking()
                    .Include(item => item.Tenant)
                    .SingleOrDefaultAsync(item => item.TenantId == tenantId && item.PortalUserId == user.Id && item.IsActive, cancellationToken);

                if (customer is null || customer.Tenant?.Status != TenantStatus.Active)
                    throw new UnauthorizedAccessException("The account has no active access to this tenant.");

                claims.Add(new Claim("tenant_id", customer.TenantId.ToString()));
                claims.Add(new Claim("customer_id", customer.Id.ToString()));
                claims.Add(new Claim("tenant_role", "Customer"));
                claims.Add(new Claim("role", "Customer"));
            }
        }

        var expiresAt = DateTimeOffset.UtcNow.AddSeconds(_options.DurationInSeconds);
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expiresAt.UtcDateTime, signingCredentials: credentials);

        return new AccessTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
