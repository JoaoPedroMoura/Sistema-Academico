using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;

namespace FaeterjAcademico.Application.Requests.TriarSolicitacao;

/// <summary>
/// Aprovar uma solicitação do tipo <c>JustificativaDeFalta</c> deveria também justificar a
/// <see cref="Domain.Entities.Presenca"/> correspondente — deixado para a Fase da área do
/// Professor/Aluno, quando lançamento de presença existir; hoje isso é feito manualmente pela
/// Secretaria fora do sistema, registrado aqui só como aprovação da solicitação em si.
/// </summary>
public sealed class AprovarSolicitacaoHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<AprovarSolicitacaoCommand, SolicitacaoDto>
{
    public async Task<SolicitacaoDto> HandleAsync(AprovarSolicitacaoCommand request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await repository.GetSolicitacaoByIdAsync(request.SolicitacaoId, cancellationToken)
            ?? throw new UseCaseException("Solicitação não encontrada.");
        var aluno = await repository.GetAlunoByIdAsync(solicitacao.AlunoId, cancellationToken)
            ?? throw new UseCaseException("Aluno da solicitação não encontrado.");

        var accountId = currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");
        solicitacao.Aprovar(accountId, request.Resposta);

        await repository.AddLogAsync(new Domain.Entities.LogSistema(
            currentUser.AccountId, "Solicitacao.Aprovar", "Solicitacao", solicitacao.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new SolicitacaoDto(
            solicitacao.Id, aluno.Id, aluno.Nome, solicitacao.Tipo.ToString(), solicitacao.Descricao,
            solicitacao.AnexoUrl, solicitacao.Status.ToString(), solicitacao.Resposta,
            solicitacao.CreatedAtUtc, solicitacao.RespondidaEmUtc);
    }
}
