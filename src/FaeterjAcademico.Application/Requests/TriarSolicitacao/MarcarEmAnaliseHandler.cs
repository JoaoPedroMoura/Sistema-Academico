using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;

namespace FaeterjAcademico.Application.Requests.TriarSolicitacao;

public sealed class MarcarEmAnaliseHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<MarcarEmAnaliseCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> HandleAsync(MarcarEmAnaliseCommand request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await repository.GetSolicitacaoByIdAsync(request.SolicitacaoId, cancellationToken)
            ?? throw new UseCaseException("Solicitação não encontrada.");
        var aluno = await repository.GetAlunoByIdAsync(solicitacao.AlunoId, cancellationToken)
            ?? throw new UseCaseException("Aluno da solicitação não encontrado.");

        solicitacao.MarcarEmAnalise();

        await repository.AddLogAsync(new Domain.Entities.LogSistema(
            currentUser.AccountId, "Solicitacao.MarcarEmAnalise", "Solicitacao", solicitacao.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new SolicitacaoDto(
            solicitacao.Id, aluno.Id, aluno.Nome, solicitacao.Tipo.ToString(), solicitacao.Descricao,
            solicitacao.AnexoUrl, solicitacao.Status.ToString(), solicitacao.Resposta,
            solicitacao.CreatedAtUtc, solicitacao.RespondidaEmUtc);
    }
}
