using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;

namespace FaeterjAcademico.Application.Requests.ListarMinhasSolicitacoes;

/// <summary>Caso de uso "Acompanhamento do status de cada solicitação" do Perfil Aluno (ANALISE-TCC.md §2).</summary>
public sealed class ListarMinhasSolicitacoesHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMinhasSolicitacoesQuery, IReadOnlyList<SolicitacaoDto>>
{
    public async Task<IReadOnlyList<SolicitacaoDto>> HandleAsync(ListarMinhasSolicitacoesQuery request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno desta unidade.");

        var solicitacoes = await repository.GetSolicitacoesByAlunoAsync(aluno.Id, cancellationToken);

        return [.. solicitacoes.Select(s => new SolicitacaoDto(
            s.Id, aluno.Id, aluno.Nome, s.Tipo.ToString(), s.Descricao, s.AnexoUrl, s.Status.ToString(),
            s.Resposta, s.CreatedAtUtc, s.RespondidaEmUtc))];
    }
}
