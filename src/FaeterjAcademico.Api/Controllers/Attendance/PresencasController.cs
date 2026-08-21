using FaeterjAcademico.Application.Attendance.Dtos;
using FaeterjAcademico.Application.Attendance.ListarMinhasPresencas;
using FaeterjAcademico.Application.Attendance.ListarPresencas;
using FaeterjAcademico.Application.Attendance.RegistrarPresenca;
using FaeterjAcademico.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Attendance;

/// <summary>Lançamento (Perfil Professor) e consulta (Perfil Aluno) de presença (ANALISE-TCC.md §2).</summary>
[ApiController]
[Route("api/presencas")]
public class PresencasController(
    RegistrarPresencaHandler registrarHandler,
    ListarPresencasHandler listarHandler,
    ListarMinhasPresencasHandler listarMinhasHandler,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    private Guid AccountIdAutenticado =>
        currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");

    [HttpGet("minhas")]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<IReadOnlyList<MinhaPresencaDto>>> ListarMinhas(CancellationToken cancellationToken) =>
        Ok(await listarMinhasHandler.HandleAsync(new ListarMinhasPresencasQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<PresencaDto>>> Listar(
        [FromQuery] Guid turmaId, [FromQuery] DateOnly data, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarPresencasQuery(turmaId, data), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<PresencaDto>>> Registrar(RegistrarPresencaRequest request, CancellationToken cancellationToken) =>
        Ok(await registrarHandler.HandleAsync(
            new RegistrarPresencaCommand(AccountIdAutenticado, request.TurmaId, request.DataAula, request.Registros),
            cancellationToken));
}
