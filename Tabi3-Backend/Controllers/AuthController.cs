using Application.Dtos.Auth;
using Application.IServices;
using Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Tabi3_Backend.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    private const string RefreshCookieName = "tabi3_refresh";

    [HttpPost("owner-registration")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> RegisterOwner(RegisterOwnerRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authService.RegisterOwnerAsync(request, cancellationToken);
        return result.IsSuccess
            ? Accepted(new { registrationRequestId = result.Data, message = result.Msg })
            : BadRequest(new { code = result.Error.Code, message = result.Error.Description });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Login(LoginRequestDto request, CancellationToken cancellationToken)
    {
        var result = await authService.LoginAsync(request.Email, request.Password, request.TenantId, cancellationToken);
        if (!result.IsSuccess)
            return Unauthorized(new { code = result.Error.Code, message = result.Error.Description });

        var authModel = result.Data!;
        if (authModel.IsAuthenticated)
            SetRefreshCookie(authModel.RefreshToken, authModel.RefreshTokenExpiration);

        return Ok(new
        {
            authModel.Id,
            authModel.Username,
            authModel.Email,
            authModel.IsAuthenticated,
            authModel.Message,
            authModel.Token,
            authModel.ExpiresOn,
            authModel.Roles,
            authModel.TenantId,
            authModel.TenantSlug,
            authModel.AvailableTenants
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [EnableRateLimiting("AuthPolicy")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        var result = await authService.RefreshSessionAsync(refreshToken ?? string.Empty, cancellationToken);
        if (!result.IsSuccess || result.Data is null)
        {
            DeleteRefreshCookie();
            return Unauthorized(new { code = result.Error.Code, message = result.Error.Description });
        }

        SetRefreshCookie(result.Data.RefreshToken, result.Data.RefreshTokenExpiration);
        return Ok(new
        {
            result.Data.Token,
            result.Data.ExpiresOn,
            result.Data.TenantId,
            result.Data.TenantSlug,
            result.Data.Roles
        });
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        await authService.LogoutAsync(Request.Cookies[RefreshCookieName] ?? string.Empty, cancellationToken);
        DeleteRefreshCookie();
        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await authService.GetCurrentUserAsync(cancellationToken);
        return result.IsSuccess
            ? Ok(result.Data)
            : Unauthorized(new { code = result.Error.Code, message = result.Error.Description });
    }

    private void SetRefreshCookie(string? token, DateTime expiration)
    {
        if (string.IsNullOrWhiteSpace(token))
            return;

        Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = new DateTimeOffset(DateTime.SpecifyKind(expiration, DateTimeKind.Utc)),
            Path = "/api/auth"
        });
    }

    private void DeleteRefreshCookie() => Response.Cookies.Delete(RefreshCookieName, new CookieOptions
    {
        HttpOnly = true,
        Secure = Request.IsHttps,
        SameSite = SameSiteMode.Strict,
        Path = "/api/auth"
    });
}
