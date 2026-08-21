using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Requests.AbrirSolicitacao;

/// <summary>
/// Caso de uso do Perfil Aluno (ANALISE-TCC.md §2) — implementado já na Fase da Secretaria
/// porque a triagem não tem o que triar sem isso; a tela de abrir solicitação em si é da área do
/// Aluno.
/// </summary>
public sealed class AbrirSolicitacaoHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AbrirSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> HandleAsync(AbrirSolicitacaoCommand request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByIdAsync(request.AlunoId, cancellationToken)
            ?? throw new UseCaseException("Aluno não encontrado.");

        var solicitacao = new Solicitacao(aluno.Id, request.Tipo, request.Descricao, request.AnexoUrl);
        await repository.AddSolicitacaoAsync(solicitacao, cancellationToken);

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Solicitacao.Abrir", "Solicitacao", solicitacao.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new SolicitacaoDto(
            solicitacao.Id, aluno.Id, aluno.Nome, solicitacao.Tipo.ToString(), solicitacao.Descricao,
            solicitacao.AnexoUrl, solicitacao.Status.ToString(), solicitacao.Resposta,
            solicitacao.CreatedAtUtc, solicitacao.RespondidaEmUtc);
    }
}
