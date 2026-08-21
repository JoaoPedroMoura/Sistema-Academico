using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.AtualizarProfessor;
using FaeterjAcademico.Application.Teachers.CriarProfessor;
using FaeterjAcademico.Application.Teachers.Dtos;
using FaeterjAcademico.Application.Teachers.ExcluirProfessor;
using FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;
using FaeterjAcademico.Application.Teachers.ListarAlunosDaTurma;
using FaeterjAcademico.Application.Teachers.ListarMinhasTurmas;
using FaeterjAcademico.Application.Teachers.ListarProfessores;
using FaeterjAcademico.Application.Teachers.ObterMeuPerfil;
using FaeterjAcademico.Application.Teachers.ObterProfessor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Teachers;

/// <summary>Caso de uso "Manter Professores" do TCC original (ANALISE-TCC.md §4, UC1).</summary>
[ApiController]
[Route("api/professores")]
public class ProfessoresController(
    ListarProfessoresHandler listarHandler,
    ObterProfessorHandler obterHandler,
    CriarProfessorHandler criarHandler,
    AtualizarProfessorHandler atualizarHandler,
    ExcluirProfessorHandler excluirHandler,
    AdicionarDisponibilidadeHandler adicionarDisponibilidadeHandler,
    RemoverDisponibilidadeHandler removerDisponibilidadeHandler,
    ObterMeuPerfilHandler obterMeuPerfilHandler,
    ListarMinhasTurmasHandler listarMinhasTurmasHandler,
    ListarAlunosDaTurmaHandler listarAlunosDaTurmaHandler,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    private Guid AccountIdAutenticado =>
        currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");

    // Self-service do próprio professor (ANALISE-TCC.md §6 — evolução em relação ao TCC
    // original, onde disponibilidade só era consumida internamente pelo GRASP).
    [HttpGet("me")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<ProfessorDto>> ObterMeuPerfil(CancellationToken cancellationToken) =>
        Ok(await obterMeuPerfilHandler.HandleAsync(new ObterMeuPerfilQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet("me/turmas")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<MinhaTurmaDto>>> ListarMinhasTurmas(CancellationToken cancellationToken) =>
        Ok(await listarMinhasTurmasHandler.HandleAsync(new ListarMinhasTurmasQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet("me/turmas/{turmaId:guid}/alunos")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<AlunoResumoDto>>> ListarAlunosDaTurma(Guid turmaId, CancellationToken cancellationToken) =>
        Ok(await listarAlunosDaTurmaHandler.HandleAsync(new ListarAlunosDaTurmaQuery(AccountIdAutenticado, turmaId), cancellationToken));

    [HttpPost("me/disponibilidades")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<ProfessorDto>> AdicionarMinhaDisponibilidade(
        AdicionarDisponibilidadeRequest request, CancellationToken cancellationToken)
    {
        var meuPerfil = await obterMeuPerfilHandler.HandleAsync(new ObterMeuPerfilQuery(AccountIdAutenticado), cancellationToken);
        return Ok(await adicionarDisponibilidadeHandler.HandleAsync(
            new AdicionarDisponibilidadeCommand(meuPerfil.Id, request.Dia, request.HoraInicio, request.HoraFim), cancellationToken));
    }

    [HttpDelete("me/disponibilidades/{disponibilidadeId:guid}")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<ProfessorDto>> RemoverMinhaDisponibilidade(
        Guid disponibilidadeId, CancellationToken cancellationToken)
    {
        var meuPerfil = await obterMeuPerfilHandler.HandleAsync(new ObterMeuPerfilQuery(AccountIdAutenticado), cancellationToken);
        return Ok(await removerDisponibilidadeHandler.HandleAsync(
            new RemoverDisponibilidadeCommand(meuPerfil.Id, disponibilidadeId), cancellationToken));
    }

    // Leitura liberada também pra Secretaria (precisa listar professores para montar o vínculo
    // matéria-professor); escrita continua exclusiva do Admin.
    [HttpGet]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<IReadOnlyList<ProfessorDto>>> Listar([FromQuery] string? pesquisa, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarProfessoresQuery(pesquisa), cancellationToken));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<ProfessorDto>> Obter(Guid id, CancellationToken cancellationToken)
    {
        var professor = await obterHandler.HandleAsync(new ObterProfessorQuery(id), cancellationToken);
        return professor is null ? NotFound() : Ok(professor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProfessorCriadoDto>> Criar(CriarProfessorRequest request, CancellationToken cancellationToken)
    {
        var resultado = await criarHandler.HandleAsync(
            new CriarProfessorCommand(request.Nome, request.Email, request.Telefone), cancellationToken);

        return CreatedAtAction(nameof(Obter), new { id = resultado.Professor.Id }, resultado);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProfessorDto>> Atualizar(Guid id, AtualizarProfessorRequest request, CancellationToken cancellationToken) =>
        Ok(await atualizarHandler.HandleAsync(
            new AtualizarProfessorCommand(id, request.Nome, request.Email, request.Telefone), cancellationToken));

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken cancellationToken)
    {
        await excluirHandler.HandleAsync(new ExcluirProfessorCommand(id), cancellationToken);
        return NoContent();
    }

    // Disponibilidade: hoje o Admin gerencia; vira self-service do próprio Professor quando a
    // área dele existir (ANALISE-TCC.md §6) — nesse momento troca pra [Authorize(Roles="Professor")]
    // com o professorId resolvido da conta autenticada, igual ao padrão usado em Solicitacoes.
    [HttpPost("{id:guid}/disponibilidades")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProfessorDto>> AdicionarDisponibilidade(
        Guid id, AdicionarDisponibilidadeRequest request, CancellationToken cancellationToken) =>
        Ok(await adicionarDisponibilidadeHandler.HandleAsync(
            new AdicionarDisponibilidadeCommand(id, request.Dia, request.HoraInicio, request.HoraFim), cancellationToken));

    [HttpDelete("{id:guid}/disponibilidades/{disponibilidadeId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProfessorDto>> RemoverDisponibilidade(
        Guid id, Guid disponibilidadeId, CancellationToken cancellationToken) =>
        Ok(await removerDisponibilidadeHandler.HandleAsync(
            new RemoverDisponibilidadeCommand(id, disponibilidadeId), cancellationToken));
}
