using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.AtualizarAluno;
using FaeterjAcademico.Application.Students.CriarAluno;
using FaeterjAcademico.Application.Students.Dtos;
using FaeterjAcademico.Application.Students.ListarAlunos;
using FaeterjAcademico.Application.Students.ObterMeuPerfilAluno;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Students;

/// <summary>Matrícula de alunos — responsabilidade da Secretaria (ARCHITECTURE.md, decisão da Fase 7).</summary>
[ApiController]
[Route("api/alunos")]
public class AlunosController(
    ListarAlunosHandler listarHandler,
    CriarAlunoHandler criarHandler,
    AtualizarAlunoHandler atualizarHandler,
    ObterMeuPerfilAlunoHandler obterMeuPerfilHandler,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    private Guid AccountIdAutenticado =>
        currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");

    [HttpGet("me")]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<AlunoDto>> ObterMeuPerfil(CancellationToken cancellationToken) =>
        Ok(await obterMeuPerfilHandler.HandleAsync(new ObterMeuPerfilAlunoQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<IReadOnlyList<AlunoDto>>> Listar([FromQuery] string? pesquisa, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarAlunosQuery(pesquisa), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<AlunoMatriculadoDto>> Matricular(CriarAlunoRequest request, CancellationToken cancellationToken)
    {
        var resultado = await criarHandler.HandleAsync(
            new CriarAlunoCommand(request.Nome, request.Email, request.Matricula, request.PeriodoAtual), cancellationToken);

        return CreatedAtAction(nameof(Listar), resultado);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<AlunoDto>> Atualizar(Guid id, AtualizarAlunoRequest request, CancellationToken cancellationToken) =>
        Ok(await atualizarHandler.HandleAsync(new AtualizarAlunoCommand(id, request.NovoPeriodo), cancellationToken));
}
