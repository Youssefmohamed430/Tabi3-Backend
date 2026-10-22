namespace Infrastructure.Identity;

public static class SuperAdminBootstrapper
{
    public static async Task EnsureSuperAdminAsync(
        this IServiceProvider services,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var userName = configuration["BootstrapSuperAdmin:UserName"];
        var email = configuration["BootstrapSuperAdmin:Email"];
        var password = configuration["BootstrapSuperAdmin:Password"];
        var phone = configuration["BootstrapSuperAdmin:Phone"];

        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password))
            return;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "Both BootstrapSuperAdmin:Email and BootstrapSuperAdmin:Password must be configured together.");

        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(DomainConstants.IdentityRoles.SuperAdmin))
            throw new InvalidOperationException(
                "The SuperAdmin Identity role is missing. Apply the database migrations before bootstrapping the account.");

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                DisplayName = configuration["BootstrapSuperAdmin:DisplayName"] ?? "Super Admin",
                PreferredLanguage = "ar",
                PhoneNumber = phone,
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                throw new InvalidOperationException(
                    $"Could not create the configured SuperAdmin account: {string.Join("; ", createResult.Errors.Select(error => error.Description))}");
        }

        if (!await userManager.IsInRoleAsync(user, DomainConstants.IdentityRoles.SuperAdmin))
        {
            var addRoleResult = await userManager.AddToRoleAsync(user, DomainConstants.IdentityRoles.SuperAdmin);
            if (!addRoleResult.Succeeded)
                throw new InvalidOperationException(
                    $"Could not assign the SuperAdmin role: {string.Join("; ", addRoleResult.Errors.Select(error => error.Description))}");
        }
    }
}
