namespace Application.Services;

public sealed class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IRepository<TenantMembership> memberships,
    IRepository<TenantRegistrationRequest> registrationRequests,
    IRepository<Tenant> tenants,
    IRepository<Customer> customers,
    IRepository<RefreshToken> refreshTokens,
    IUnitOfWork unitOfWork,
    IAccessTokenService accessTokenService,
    IHttpContextAccessor httpContextAccessor) : IAuthService
{
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<Result<Guid>> RegisterOwnerAsync(RegisterOwnerRequestDto request, CancellationToken cancellationToken = default)
    {
        var ownerName = request.OwnerName.Trim();
        var username = request.Username.Trim();
        var email = request.Email.Trim();
        var phone = request.Phone.Trim();
        var businessName = request.BusinessName.Trim();
        var slug = NormalizeSlug(request.TenantSlug);

        if (ownerName.Length is 0 or > 200 || username.Length is 0 or > 256 ||
            email.Length is 0 or > 254 || phone.Length is 0 or > 32 ||
            businessName.Length is 0 or > 200 ||
            !IsValidSlug(slug) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<Guid>(new Error("Auth.InvalidRegistration", "بيانات التسجيل غير مكتملة أو غير صالحة."));
        }

        if (await userManager.FindByEmailAsync(email) is not null)
            return Result.Failure<Guid>(new Error("Auth.RegistrationConflict", "تعذر إنشاء الحساب بهذه البيانات."));

        if (await userManager.FindByNameAsync(username) is not null)
            return Result.Failure<Guid>(new Error("Auth.RegistrationConflict", "تعذر إنشاء الحساب بهذه البيانات."));

        if (await tenants.FirstOrDefaultAsync(new TenantBySlugSpecification(slug), cancellationToken) is not null ||
            await registrationRequests.FirstOrDefaultAsync(new TenantRegistrationBySlugSpecification(slug), cancellationToken) is not null)
        {
            return Result.Failure<Guid>(new Error("Auth.TenantSlugUnavailable", "رابط المطبعة غير متاح."));
        }

        var owner = new ApplicationUser
        {
            UserName = username,
            Email = email,
            PhoneNumber = phone,
            PhoneNormalized = NormalizePhone(phone),
            DisplayName = ownerName,
            PreferredLanguage = "ar",
            CreatedAt = EgyptianDateTime.Now
        };

        var creation = await userManager.CreateAsync(owner, request.Password);
        if (!creation.Succeeded)
        {
            return Result.Failure<Guid>(new Error("Auth.AccountCreationFailed", "تعذر إنشاء الحساب بهذه البيانات."));
        }

        var registration = new TenantRegistrationRequest
        {
            BusinessName = businessName,
            Slug = slug,
            OwnerUserId = owner.Id,
            Phone = phone,
            Email = email,
            Status = TenantRegistrationStatus.Submitted,
            CreatedAt = EgyptianDateTime.Now
        };

        try
        {
            await registrationRequests.AddAsync(registration, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await userManager.DeleteAsync(owner);
            throw;
        }

        return Result.Success(registration.Id, "تم استلام طلب تسجيل المطبعة وبيانات المالك لمراجعته من مسؤول المنصة.");
    }

    public async Task<Result<AuthModel>> LoginAsync(string email, string password, Guid? tenantId = null, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email.Trim());
        if (user is null || !(await signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true)).Succeeded)
            return InvalidCredentials();

        var choices = await GetAvailableTenantsAsync(user.Id, cancellationToken);
        if (tenantId is null && choices.Count > 1)
        {
            return Result.Success(new AuthModel
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                AvailableTenants = choices,
                IsAuthenticated = false,
                Message = "اختر المطبعة المرتبطة بحسابك لإكمال تسجيل الدخول."
            });
        }

        var selectedTenantId = tenantId ?? choices.SingleOrDefault()?.TenantId;
        if (selectedTenantId is null)
        {
            var applicationRoles = await userManager.GetRolesAsync(user);
            if (applicationRoles.Contains(DomainConstants.IdentityRoles.SuperAdmin, StringComparer.Ordinal))
                return await IssueSessionAsync(user, null, cancellationToken);

            var pendingRegistration = await registrationRequests.FirstOrDefaultAsync(
                new PendingTenantRegistrationByOwnerSpecification(user.Id), cancellationToken);
            if (pendingRegistration is not null)
                return Result.Failure<AuthModel>(new Error("Auth.TenantPendingReview", "طلب تسجيل المطبعة قيد مراجعة مسؤول المنصة."));

            return Result.Failure<AuthModel>(new Error("Auth.NoActiveTenantAccess", "الحساب لا يملك وصولًا نشطًا إلى مطبعة."));
        }

        var matchingChoice = choices.SingleOrDefault(option => option.TenantId == selectedTenantId);
        if (matchingChoice is null)
            return Result.Failure<AuthModel>(new Error("Auth.InvalidTenantSelection", "الحساب لا يملك وصولًا إلى المطبعة المحددة."));

        return await IssueSessionAsync(user, selectedTenantId, cancellationToken);
    }

    public async Task<Result<AuthModel>> RefreshSessionAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Failure<AuthModel>(new Error("Auth.InvalidRefreshToken", "الجلسة غير صالحة أو انتهت."));

        var stored = await refreshTokens.FirstOrDefaultAsync(new RefreshTokenByHashSpecification(HashToken(refreshToken)), cancellationToken);
        var now = DateTimeOffset.UtcNow;
        if (stored is null || stored.RevokedAt is not null || stored.ExpiresAt <= now)
            return Result.Failure<AuthModel>(new Error("Auth.InvalidRefreshToken", "الجلسة غير صالحة أو انتهت."));

        var user = await userManager.FindByIdAsync(stored.UserId);
        if (user is null || (user.LockoutEnd is not null && user.LockoutEnd > now))
            return Result.Failure<AuthModel>(new Error("Auth.InvalidRefreshToken", "الجلسة غير صالحة أو انتهت."));

        if (stored.TenantId is Guid tenantId)
        {
            var choices = await GetAvailableTenantsAsync(user.Id, cancellationToken);
            if (!choices.Any(option => option.TenantId == tenantId))
                return Result.Failure<AuthModel>(new Error("Auth.InvalidRefreshToken", "الجلسة غير صالحة أو انتهت."));
        }
        else if (!(await userManager.GetRolesAsync(user)).Contains(DomainConstants.IdentityRoles.SuperAdmin, StringComparer.Ordinal))
        {
            return Result.Failure<AuthModel>(new Error("Auth.InvalidRefreshToken", "الجلسة غير صالحة أو انتهت."));
        }

        stored.RevokedAt = now;
        var nextToken = CreateRefreshToken(user.Id, stored.TenantId);
        stored.ReplacedByTokenHash = nextToken.Entity.TokenHash;
        refreshTokens.Update(stored);
        await refreshTokens.AddAsync(nextToken.Entity, cancellationToken);

        var accessToken = await accessTokenService.CreateAsync(user.Id, stored.TenantId, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success(CreateAuthModel(user, accessToken, nextToken.RawToken, stored.TenantId));
    }

    public async Task<Result> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Result.Success("تم تسجيل الخروج.");

        var stored = await refreshTokens.FirstOrDefaultAsync(new RefreshTokenByHashSpecification(HashToken(refreshToken)), cancellationToken);
        if (stored is not null && stored.RevokedAt is null)
        {
            stored.RevokedAt = DateTimeOffset.UtcNow;
            refreshTokens.Update(stored);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success("تم تسجيل الخروج.");
    }

    public async Task<Result<AuthModel>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Result.Failure<AuthModel>(new Error("Auth.Unauthenticated", "يلزم تسجيل الدخول."));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthModel>(new Error("Auth.Unauthenticated", "يلزم تسجيل الدخول."));

        var tenantClaim = httpContextAccessor.HttpContext?.User.FindFirst("tenant_id")?.Value;
        Guid? tenantId = Guid.TryParse(tenantClaim, out var parsedTenantId) ? parsedTenantId : null;
        return Result.Success(new AuthModel
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            TenantId = tenantId,
            IsAuthenticated = true,
            Roles = httpContextAccessor.HttpContext?.User.FindAll("role").Select(claim => claim.Value).ToList() ?? []
        });
    }

    public async Task<Result<AuthModel>> SelectTenantAsync(Guid membershipId, CancellationToken cancellationToken = default)
    {
        var userId = GetCurrentUserId();
        if (userId is null)
            return Result.Failure<AuthModel>(new Error("Auth.Unauthenticated", "يلزم تسجيل الدخول."));

        var membership = await memberships.FirstOrDefaultAsync(new ActiveMembershipByIdAndUserSpecification(membershipId, userId), cancellationToken);
        if (membership is null)
            return Result.Failure<AuthModel>(new Error("Auth.InvalidTenantSelection", "الحساب لا يملك وصولًا نشطًا إلى المطبعة المحددة."));

        var user = await userManager.FindByIdAsync(userId);
        return user is null
            ? Result.Failure<AuthModel>(new Error("Auth.Unauthenticated", "يلزم تسجيل الدخول."))
            : await IssueSessionAsync(user, membership.TenantId, cancellationToken);
    }

    public Task<Result> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(new Error("Auth.PasswordResetUnavailable", "إرسال رسائل استعادة كلمة المرور غير مهيأ بعد.")));

    public Task<Result> ResetPasswordAsync(string resetToken, string newPassword, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(new Error("Auth.PasswordResetUnavailable", "إرسال رسائل استعادة كلمة المرور غير مهيأ بعد.")));

    public Task<Result> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(new Error("Auth.PasswordChangeUnavailable", "تغيير كلمة المرور غير مهيأ بعد.")));

    public Task<Result> VerifyEmailAsync(string verificationToken, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(new Error("Auth.EmailVerificationUnavailable", "التحقق من البريد الإلكتروني غير مهيأ بعد.")));

    public Task<Result<AuthModel>> AcceptInvitationAsync(string invitationToken, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure<AuthModel>(new Error("Auth.InvitationUnavailable", "دعوات أعضاء المطابع ستنفذ ضمن مرحلة إدارة الأعضاء.")));

    public Task<Result> CompleteAccountSetupAsync(string accountSetupToken, string password, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(new Error("Auth.AccountSetupUnavailable", "إعداد حساب العضو سينفذ ضمن مرحلة إدارة الأعضاء.")));

    private async Task<Result<AuthModel>> IssueSessionAsync(ApplicationUser user, Guid? tenantId, CancellationToken cancellationToken)
    {
        var accessToken = await accessTokenService.CreateAsync(user.Id, tenantId, cancellationToken);
        var token = CreateRefreshToken(user.Id, tenantId);
        await refreshTokens.AddAsync(token.Entity, cancellationToken);
        user.LastLoginAt = EgyptianDateTime.Now;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        var authModel = CreateAuthModel(user, accessToken, token.RawToken, tenantId);
        if (tenantId is null)
        {
            authModel.Roles = (await userManager.GetRolesAsync(user)).ToList();
        }
        else
        {
            var options = await GetAvailableTenantsAsync(user.Id, cancellationToken);
            authModel.Roles = [options.Single(option => option.TenantId == tenantId).RoleCode];
            authModel.TenantSlug = options.Single(option => option.TenantId == tenantId).TenantSlug;
        }
        return Result.Success(authModel);
    }

    private async Task<List<TenantLoginOption>> GetAvailableTenantsAsync(string userId, CancellationToken cancellationToken)
    {
        var activeMemberships = await memberships.ListAsync(new ActiveMembershipsByUserSpecification(userId), cancellationToken);
        var options = activeMemberships
            .Where(membership => membership.Tenant is { Status: TenantStatus.Active })
            .Select(membership => new TenantLoginOption(membership.TenantId, membership.Tenant!.Name, membership.Tenant.Slug, membership.Role!.Code))
            .ToList();

        var activeCustomers = await customers.ListAsync(new ActiveCustomerAccountsByUserSpecification(userId), cancellationToken);
        options.AddRange(activeCustomers
            .Where(customer => customer.Tenant is { Status: TenantStatus.Active })
            .Select(customer => new TenantLoginOption(customer.TenantId, customer.Tenant!.Name, customer.Tenant.Slug, "Customer")));
        return options;
    }

    private static AuthModel CreateAuthModel(ApplicationUser user, AccessTokenResult accessToken, string refreshToken, Guid? tenantId) => new()
    {
        Id = user.Id,
        Username = user.UserName,
        Email = user.Email,
        IsAuthenticated = true,
        Token = accessToken.Token,
        ExpiresOn = accessToken.ExpiresAt.UtcDateTime,
        RefreshToken = refreshToken,
        RefreshTokenExpiration = DateTime.UtcNow.Add(RefreshTokenLifetime),
        TenantId = tenantId
    };

    private (RefreshToken Entity, string RawToken) CreateRefreshToken(string userId, Guid? tenantId)
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Convert.ToBase64String(randomBytes);
        return (new RefreshToken
        {
            UserId = userId,
            TenantId = tenantId,
            TokenHash = HashToken(rawToken),
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.Add(RefreshTokenLifetime),
            CreatedByIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString()
        }, rawToken);
    }

    private static string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

    private static string NormalizeSlug(string value) => value.Trim().ToLowerInvariant();

    private static bool IsValidSlug(string value) =>
        value.Length is > 0 and <= 100 &&
        System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$");

    private static string NormalizePhone(string phone) => new(phone.Where(char.IsDigit).ToArray());

    private string? GetCurrentUserId() =>
        httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
        httpContextAccessor.HttpContext?.User.FindFirst("sub")?.Value;

    private static Result<AuthModel> InvalidCredentials() =>
        Result.Failure<AuthModel>(new Error("Auth.InvalidCredentials", "البريد الإلكتروني أو كلمة المرور غير صحيحة."));
}
