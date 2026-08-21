using FaeterjAcademico.Application.Schedule.Dtos;
using FaeterjAcademico.Application.Schedule.GerarGrade;
using FaeterjAcademico.Application.Schedule.ListarGrades;
using FaeterjAcademico.Application.Schedule.ObterGradeAtiva;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Schedule;

/// <summary>Caso de uso "Manter Grade de Horário" do TCC original (ANALISE-TCC.md §4, UC7).</summary>
[ApiController]
[Route("api/grades")]
[Authorize(Roles = "Admin,Secretaria")]
public class GradesController(
    GerarGradeHandler gerarHandler,
    ObterGradeAtivaHandler obterAtivaHandler,
    ListarGradesHandler listarHandler) : ControllerBase
{
    [HttpPost("gerar")]
    public async Task<ActionResult<GerarGradeResultDto>> Gerar([FromQuery] int? iterations, CancellationToken cancellationToken) =>
        Ok(await gerarHandler.HandleAsync(new GerarGradeCommand(iterations), cancellationToken));

    [HttpGet("ativa")]
    public async Task<ActionResult<GradeDto>> ObterAtiva(CancellationToken cancellationToken)
    {
        var grade = await obterAtivaHandler.HandleAsync(new ObterGradeAtivaQuery(), cancellationToken);
        return grade is null ? NotFound() : Ok(grade);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<GradeResumoDto>>> Listar(CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarGradesQuery(), cancellationToken));
}
