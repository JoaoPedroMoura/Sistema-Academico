using FaeterjAcademico.Application.Auth.Login;
using FaeterjAcademico.Application.Auth.Logout;
using FaeterjAcademico.Application.Auth.Refresh;
using FaeterjAcademico.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Auth;

/// <summary>
/// Login, refresh e logout (ARCHITECTURE.md §4). O refresh token vive num cookie httpOnly
/// (nunca no corpo da resposta) — só o access token de curta duração vai para o JSON, guardado
/// em memória pelo frontend.
/// </summary>
[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController(
    LoginHandler loginHandler,
    RefreshTokenHandler refreshTokenHandler,
    LogoutHandler logoutHandler,
    IWebHostEnvironment environment) : ControllerBase
{
    private const string RefreshCookieName = "faeterj_refresh";

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

        LoginResult result;
        try
        {
            result = await loginHandler.HandleAsync(
                new LoginCommand(request.Email, request.Password, request.TenantSlug, ip),
                cancellationToken);
        }
        catch (AuthenticationFailedException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }

        if (result.Status == LoginStatus.PrecisaEscolherTenant)
        {
            return Ok(new LoginResponse(
                RequiresTenantSelection: true,
                TenantOptions: [.. result.OpcoesDeTenant.Select(o => new TenantOptionResponse(o.Slug, o.Nome, o.Role.ToString()))],
                AccessToken: null,
                AccessTokenExpiresAtUtc: null,
                AccountId: null,
                Nome: null,
                Email: null,
                TenantSlug: null,
                TenantNome: null,
                Role: null,
                PrecisaTrocarSenha: false));
        }

        SetRefreshCookie(result.RefreshToken!, result.RefreshTokenExpiresAtUtc!.Value);

        return Ok(new LoginResponse(
            RequiresTenantSelection: false,
            TenantOptions: [],
            AccessToken: result.AccessToken,
            AccessTokenExpiresAtUtc: result.AccessTokenExpiresAtUtc,
            AccountId: result.AccountId,
            Nome: result.Nome,
            Email: result.Email,
            TenantSlug: result.TenantSlug,
            TenantNome: result.TenantNome,
            Role: result.Role?.ToString(),
            PrecisaTrocarSenha: result.PrecisaTrocarSenha));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var rawToken) || string.IsNullOrEmpty(rawToken))
        {
            return Unauthorized(new { message = "Sessão não encontrada." });
        }

        RefreshTokenResult result;
        try
        {
            result = await refreshTokenHandler.HandleAsync(new RefreshTokenCommand(rawToken), cancellationToken);
        }
        catch (AuthenticationFailedException ex)
        {
            DeleteRefreshCookie();
            return Unauthorized(new { message = ex.Message });
        }

        SetRefreshCookie(result.RefreshToken, result.RefreshTokenExpiresAtUtc);

        return Ok(new LoginResponse(
            RequiresTenantSelection: false,
            TenantOptions: [],
            AccessToken: result.AccessToken,
            AccessTokenExpiresAtUtc: result.AccessTokenExpiresAtUtc,
            AccountId: result.AccountId,
            Nome: result.Nome,
            Email: result.Email,
            TenantSlug: result.TenantSlug,
            TenantNome: result.TenantNome,
            Role: result.Role.ToString(),
            PrecisaTrocarSenha: result.PrecisaTrocarSenha));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var rawToken) && !string.IsNullOrEmpty(rawToken))
        {
            await logoutHandler.HandleAsync(new LogoutCommand(rawToken), cancellationToken);
        }

        DeleteRefreshCookie();
        return NoContent();
    }

    private void SetRefreshCookie(string token, DateTime expiresAtUtc) =>
        Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Expires = expiresAtUtc,
            Path = "/api/auth",
        });

    private void DeleteRefreshCookie() =>
        Response.Cookies.Delete(RefreshCookieName, new CookieOptions { Path = "/api/auth" });
}
