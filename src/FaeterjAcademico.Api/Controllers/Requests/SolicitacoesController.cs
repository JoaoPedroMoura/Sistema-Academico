using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.AbrirSolicitacao;
using FaeterjAcademico.Application.Requests.Dtos;
using FaeterjAcademico.Application.Requests.ListarMinhasSolicitacoes;
using FaeterjAcademico.Application.Requests.ListarSolicitacoes;
using FaeterjAcademico.Application.Requests.TriarSolicitacao;
using FaeterjAcademico.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Requests;

/// <summary>
/// Perfil Aluno abre (ANALISE-TCC.md §2); Secretaria/Admin triam (aprovar/rejeitar/em análise).
/// Sem [Authorize] de classe — cada ação declara seu próprio papel, porque os dois lados desta
/// tela têm papéis mutuamente exclusivos.
/// </summary>
[ApiController]
[Route("api/solicitacoes")]
public class SolicitacoesController(
    AbrirSolicitacaoHandler abrirHandler,
    ListarSolicitacoesHandler listarHandler,
    ListarMinhasSolicitacoesHandler listarMinhasHandler,
    MarcarEmAnaliseHandler marcarEmAnaliseHandler,
    AprovarSolicitacaoHandler aprovarHandler,
    RejeitarSolicitacaoHandler rejeitarHandler,
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    [HttpGet("minhas")]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> ListarMinhas(CancellationToken cancellationToken)
    {
        var accountId = currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");
        return Ok(await listarMinhasHandler.HandleAsync(new ListarMinhasSolicitacoesQuery(accountId), cancellationToken));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<IReadOnlyList<SolicitacaoDto>>> Listar(
        [FromQuery] StatusSolicitacao? status, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarSolicitacoesQuery(status), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<SolicitacaoDto>> Abrir(AbrirSolicitacaoRequest request, CancellationToken cancellationToken)
    {
        var accountId = currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");
        var aluno = await repository.GetAlunoByAccountIdAsync(accountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno matriculado nesta unidade.");

        var solicitacao = await abrirHandler.HandleAsync(
            new AbrirSolicitacaoCommand(aluno.Id, request.Tipo, request.Descricao, request.AnexoUrl), cancellationToken);

        return CreatedAtAction(nameof(Listar), solicitacao);
    }

    [HttpPost("{id:guid}/em-analise")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<SolicitacaoDto>> MarcarEmAnalise(Guid id, CancellationToken cancellationToken) =>
        Ok(await marcarEmAnaliseHandler.HandleAsync(new MarcarEmAnaliseCommand(id), cancellationToken));

    [HttpPost("{id:guid}/aprovar")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<SolicitacaoDto>> Aprovar(Guid id, AprovarSolicitacaoRequest request, CancellationToken cancellationToken) =>
        Ok(await aprovarHandler.HandleAsync(new AprovarSolicitacaoCommand(id, request.Resposta), cancellationToken));

    [HttpPost("{id:guid}/rejeitar")]
    [Authorize(Roles = "Admin,Secretaria")]
    public async Task<ActionResult<SolicitacaoDto>> Rejeitar(Guid id, RejeitarSolicitacaoRequest request, CancellationToken cancellationToken) =>
        Ok(await rejeitarHandler.HandleAsync(new RejeitarSolicitacaoCommand(id, request.Resposta), cancellationToken));
}
