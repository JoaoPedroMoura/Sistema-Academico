using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Requests.Dtos;

namespace FaeterjAcademico.Application.Requests.ListarSolicitacoes;

public sealed class ListarSolicitacoesHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarSolicitacoesQuery, IReadOnlyList<SolicitacaoDto>>
{
    public async Task<IReadOnlyList<SolicitacaoDto>> HandleAsync(ListarSolicitacoesQuery request, CancellationToken cancellationToken = default)
    {
        var solicitacoes = await repository.GetSolicitacoesAsync(request.Status, cancellationToken);
        var alunos = (await repository.GetAlunosAsync(cancellationToken)).ToDictionary(a => a.Id);

        return [.. solicitacoes
            .Where(s => alunos.ContainsKey(s.AlunoId))
            .Select(s => new SolicitacaoDto(
                s.Id,
                s.AlunoId,
                alunos[s.AlunoId].Nome,
                s.Tipo.ToString(),
                s.Descricao,
                s.AnexoUrl,
                s.Status.ToString(),
                s.Resposta,
                s.CreatedAtUtc,
                s.RespondidaEmUtc))];
    }
}
