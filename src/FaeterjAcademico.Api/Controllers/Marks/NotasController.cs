using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Marks.Dtos;
using FaeterjAcademico.Application.Marks.LancarNota;
using FaeterjAcademico.Application.Marks.ListarMinhasNotas;
using FaeterjAcademico.Application.Marks.ListarNotas;
using FaeterjAcademico.Application.Marks.RevisarNota;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Marks;

/// <summary>Lançamento (Perfil Professor) e consulta (Perfil Aluno) de notas (ANALISE-TCC.md §2).</summary>
[ApiController]
[Route("api/notas")]
public class NotasController(
    LancarNotaHandler lancarHandler,
    RevisarNotaHandler revisarHandler,
    ListarNotasHandler listarHandler,
    ListarMinhasNotasHandler listarMinhasHandler,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    private Guid AccountIdAutenticado =>
        currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");

    [HttpGet("minhas")]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<IReadOnlyList<MinhaNotaDto>>> ListarMinhas(CancellationToken cancellationToken) =>
        Ok(await listarMinhasHandler.HandleAsync(new ListarMinhasNotasQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<NotaDto>>> Listar([FromQuery] Guid turmaId, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarNotasQuery(turmaId), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<NotaDto>> Lancar(LancarNotaRequest request, CancellationToken cancellationToken)
    {
        var nota = await lancarHandler.HandleAsync(
            new LancarNotaCommand(AccountIdAutenticado, request.TurmaId, request.AlunoId, request.Tipo, request.Valor),
            cancellationToken);

        return CreatedAtAction(nameof(Listar), new { turmaId = request.TurmaId }, nota);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<NotaDto>> Revisar(Guid id, RevisarNotaRequest request, CancellationToken cancellationToken) =>
        Ok(await revisarHandler.HandleAsync(new RevisarNotaCommand(AccountIdAutenticado, id, request.NovoValor), cancellationToken));
}
