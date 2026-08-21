using FaeterjAcademico.Application.Subjects.AtualizarMateria;
using FaeterjAcademico.Application.Subjects.CriarMateria;
using FaeterjAcademico.Application.Subjects.Dtos;
using FaeterjAcademico.Application.Subjects.ExcluirMateria;
using FaeterjAcademico.Application.Subjects.ListarMaterias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Subjects;

/// <summary>Caso de uso "Manter Matérias" do TCC original (ANALISE-TCC.md §4, UC2).</summary>
[ApiController]
[Route("api/materias")]
[Authorize(Roles = "Admin,Secretaria")]
public class MateriasController(
    ListarMateriasHandler listarHandler,
    CriarMateriaHandler criarHandler,
    AtualizarMateriaHandler atualizarHandler,
    ExcluirMateriaHandler excluirHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MateriaDto>>> Listar([FromQuery] string? pesquisa, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarMateriasQuery(pesquisa), cancellationToken));

    [HttpPost]
    public async Task<ActionResult<MateriaDto>> Criar(CriarMateriaRequest request, CancellationToken cancellationToken)
    {
        var materia = await criarHandler.HandleAsync(
            new CriarMateriaCommand(request.Nome, request.Periodo, request.CargaHorariaSemanal), cancellationToken);

        return CreatedAtAction(nameof(Listar), materia);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MateriaDto>> Atualizar(Guid id, AtualizarMateriaRequest request, CancellationToken cancellationToken) =>
        Ok(await atualizarHandler.HandleAsync(
            new AtualizarMateriaCommand(id, request.Nome, request.Periodo, request.CargaHorariaSemanal), cancellationToken));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await excluirHandler.HandleAsync(new ExcluirMateriaCommand(id), cancellationToken);
        return NoContent();
    }
}
