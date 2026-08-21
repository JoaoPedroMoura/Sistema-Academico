using FaeterjAcademico.Application.TeacherSubjects.AdicionarVinculo;
using FaeterjAcademico.Application.TeacherSubjects.Dtos;
using FaeterjAcademico.Application.TeacherSubjects.ListarVinculos;
using FaeterjAcademico.Application.TeacherSubjects.RemoverVinculo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Subjects;

/// <summary>Caso de uso "Manter Matérias do Professor" do TCC original (ANALISE-TCC.md §4, UC3).</summary>
[ApiController]
[Route("api/vinculos")]
[Authorize(Roles = "Admin,Secretaria")]
public class VinculosController(
    ListarVinculosHandler listarHandler,
    AdicionarVinculoHandler adicionarHandler,
    RemoverVinculoHandler removerHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<VinculoDto>>> Listar(CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarVinculosQuery(), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<VinculoDto>> Adicionar(AdicionarVinculoRequestBody request, CancellationToken cancellationToken)
    {
        var vinculo = await adicionarHandler.HandleAsync(
            new AdicionarVinculoCommand(request.MateriaId, request.ProfessorId), cancellationToken);

        return CreatedAtAction(nameof(Listar), vinculo);
    }

    [HttpDelete]
    public async Task<IActionResult> Remover([FromQuery] Guid materiaId, [FromQuery] Guid professorId, CancellationToken cancellationToken)
    {
        await removerHandler.HandleAsync(new RemoverVinculoCommand(materiaId, professorId), cancellationToken);
        return NoContent();
    }
}

public sealed record AdicionarVinculoRequestBody(Guid MateriaId, Guid ProfessorId);
