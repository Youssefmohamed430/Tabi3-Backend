namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            services.AddDbContext<Tabi3DbContext>(options => options.UseNpgsql(connectionString));
        }

        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentTenant, HttpCurrentTenant>();
        services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<Tabi3DbContext>();
        var jwtKey = configuration["JWTKey"];
        if (string.IsNullOrWhiteSpace(jwtKey) || System.Text.Encoding.UTF8.GetByteCount(jwtKey) < 32)
            throw new InvalidOperationException("JWTKey must be configured and contain at least 32 UTF-8 bytes.");

        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Issuer) && !string.IsNullOrWhiteSpace(options.Audience) && options.DurationInSeconds is > 0 and <= 3600,
                "JWT issuer, audience, and a duration from 1 to 3600 seconds must be configured.")
            .ValidateOnStart();
        services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration[$"{JwtOptions.SectionName}:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration[$"{JwtOptions.SectionName}:Audience"],
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "sub",
                    RoleClaimType = "role"
                };
            });
        services.AddAuthorization();
        services.AddScoped<IAccessTokenService, JwtAccessTokenService>();

        var storageEndpoint = configuration["Storage:Endpoint"];
        var storageRegion = configuration["Storage:Region"];
        var storageBucket = configuration["Storage:Bucket"];
        var accessKeyId = configuration["S3:AccessKeyId"];
        var secretAccessKey = configuration["S3:SecretAccessKey"];

        var hasStorageConfiguration = new[] { storageEndpoint, storageRegion, storageBucket, accessKeyId, secretAccessKey }
            .Any(value => !string.IsNullOrWhiteSpace(value));

        if (hasStorageConfiguration)
        {
            if (!Uri.TryCreate(storageEndpoint, UriKind.Absolute, out var endpoint) || endpoint.Scheme != Uri.UriSchemeHttps ||
                string.IsNullOrWhiteSpace(storageRegion) || string.IsNullOrWhiteSpace(storageBucket) ||
                string.IsNullOrWhiteSpace(accessKeyId) || string.IsNullOrWhiteSpace(secretAccessKey))
            {
                throw new InvalidOperationException("Supabase Storage configuration is incomplete. Configure Storage:Endpoint, Storage:Region, Storage:Bucket, S3:AccessKeyId, and S3:SecretAccessKey.");
            }

            var storageOptions = new SupabaseStorageOptions
            {
                Endpoint = endpoint.AbsoluteUri.TrimEnd('/'),
                Region = storageRegion,
                Bucket = storageBucket
            };

            services.AddSingleton(storageOptions);
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var clientConfiguration = new AmazonS3Config
                {
                    ServiceURL = storageOptions.Endpoint,
                    AuthenticationRegion = storageOptions.Region,
                    ForcePathStyle = true
                };

                return new AmazonS3Client(
                    new BasicAWSCredentials(accessKeyId, secretAccessKey),
                    clientConfiguration);
            });
            services.AddSingleton<IFileStorage, SupabaseS3FileStorage>();
        }

        return services;
    }
}
