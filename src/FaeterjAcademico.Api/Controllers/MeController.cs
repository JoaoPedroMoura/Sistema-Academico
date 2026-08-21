using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers;

public sealed record MeResponse(Guid AccountId, string Nome, string Email, string TenantSlug, string TenantNome, string Role);

/// <summary>
/// Prova de ponta a ponta do pipeline de auth (ARCHITECTURE.md §4): qualquer papel autenticado
/// pode chamar, útil para o frontend restaurar a sessão após reload da página.
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController : ControllerBase
{
    [HttpGet]
    public ActionResult<MeResponse> Get()
    {
        var user = HttpContext.User;

        return Ok(new MeResponse(
            AccountId: Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub")!),
            Nome: user.FindFirstValue("name") ?? string.Empty,
            Email: user.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
            TenantSlug: user.FindFirstValue("tenant") ?? string.Empty,
            TenantNome: user.FindFirstValue("tenant_name") ?? string.Empty,
            Role: user.FindFirstValue(ClaimTypes.Role) ?? string.Empty));
    }
}
