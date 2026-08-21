using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;

namespace FaeterjAcademico.Application.Requests.TriarSolicitacao;

public sealed class RejeitarSolicitacaoHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<RejeitarSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> HandleAsync(RejeitarSolicitacaoCommand request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await repository.GetSolicitacaoByIdAsync(request.SolicitacaoId, cancellationToken)
            ?? throw new UseCaseException("Solicitação não encontrada.");
        var aluno = await repository.GetAlunoByIdAsync(solicitacao.AlunoId, cancellationToken)
            ?? throw new UseCaseException("Aluno da solicitação não encontrado.");

        var accountId = currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");
        solicitacao.Rejeitar(accountId, request.Resposta);

        await repository.AddLogAsync(new Domain.Entities.LogSistema(
            currentUser.AccountId, "Solicitacao.Rejeitar", "Solicitacao", solicitacao.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new SolicitacaoDto(
            solicitacao.Id, aluno.Id, aluno.Nome, solicitacao.Tipo.ToString(), solicitacao.Descricao,
            solicitacao.AnexoUrl, solicitacao.Status.ToString(), solicitacao.Resposta,
            solicitacao.CreatedAtUtc, solicitacao.RespondidaEmUtc);
    }
}
