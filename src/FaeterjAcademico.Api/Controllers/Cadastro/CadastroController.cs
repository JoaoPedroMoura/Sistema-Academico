using FaeterjAcademico.Application.Cadastro.VerificarDisponibilidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Cadastro;

[ApiController]
[Route("api/cadastro")]
public class CadastroController(VerificarDisponibilidadeHandler verificarHandler) : ControllerBase
{
    /// <summary>Restrito a quem cadastra: revela se um email já tem conta.</summary>
    [HttpGet("disponibilidade")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<DisponibilidadeDto>> VerificarDisponibilidade(
        [FromQuery] string email, CancellationToken cancellationToken) =>
        Ok(await verificarHandler.HandleAsync(new VerificarDisponibilidadeQuery(email), cancellationToken));
}
