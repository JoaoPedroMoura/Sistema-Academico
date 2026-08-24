using System.Security.Claims;
using FaeterjAcademico.Application.Auth.TrocarSenha;
using FaeterjAcademico.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers;

public sealed record MeResponse(Guid AccountId, string Nome, string Email, string TenantSlug, string TenantNome, string Role);

public sealed record TrocarSenhaRequest(string SenhaAtual, string NovaSenha);

/// <summary>
/// Endpoints de "minha conta" — qualquer papel autenticado pode chamar, sem restrição de Role
/// (ARCHITECTURE.md §4).
/// </summary>
[ApiController]
[Route("api/me")]
[Authorize]
public class MeController(TrocarSenhaHandler trocarSenhaHandler, ICurrentUserAccessor currentUser) : ControllerBase
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

    /// <summary>
    /// Troca de senha self-service — usado tanto pelo fluxo obrigatório de primeira senha
    /// temporária (ARCHITECTURE.md §7.5) quanto por uma troca voluntária. Sem restrição de Role:
    /// qualquer papel autenticado pode trocar a própria senha.
    /// </summary>
    [HttpPost("trocar-senha")]
    public async Task<IActionResult> TrocarSenha(TrocarSenhaRequest request, CancellationToken cancellationToken)
    {
        var accountId = currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");
        await trocarSenhaHandler.HandleAsync(
            new TrocarSenhaCommand(accountId, request.SenhaAtual, request.NovaSenha), cancellationToken);
        return NoContent();
    }
}
