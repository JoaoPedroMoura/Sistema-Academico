using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Materials.Dtos;

namespace FaeterjAcademico.Application.Materials.ListarMeusMateriais;

/// <summary>Caso de uso "Download de materiais complementares" do Perfil Aluno (ANALISE-TCC.md §2).</summary>
public sealed class ListarMeusMateriaisHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMeusMateriaisQuery, IReadOnlyList<MeuMaterialDto>>
{
    public async Task<IReadOnlyList<MeuMaterialDto>> HandleAsync(ListarMeusMateriaisQuery request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno desta unidade.");

        var turmas = await repository.GetTurmasByPeriodoAsync(aluno.PeriodoAtual, cancellationToken);
        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);

        var resultado = new List<MeuMaterialDto>();
        foreach (var turma in turmas)
        {
            if (!materias.TryGetValue(turma.MateriaId, out var materia))
            {
                continue;
            }

            var materiais = await repository.GetMateriaisByTurmaAsync(turma.Id, cancellationToken);
            resultado.AddRange(materiais.Select(m => new MeuMaterialDto(
                m.Id, m.TurmaId, materia.Nome, m.Titulo, m.Descricao, m.ArquivoUrl, m.ArquivoNomeOriginal, m.TamanhoBytes, m.CreatedAtUtc)));
        }

        return [.. resultado.OrderByDescending(m => m.EnviadoEmUtc)];
    }
}
