using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Marks.Dtos;

namespace FaeterjAcademico.Application.Marks.ListarMinhasNotas;

/// <summary>Caso de uso "Consulta de notas por disciplina/período" do Perfil Aluno (ANALISE-TCC.md §2).</summary>
public sealed class ListarMinhasNotasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMinhasNotasQuery, IReadOnlyList<MinhaNotaDto>>
{
    public async Task<IReadOnlyList<MinhaNotaDto>> HandleAsync(ListarMinhasNotasQuery request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno desta unidade.");

        var notas = await repository.GetNotasByAlunoAsync(aluno.Id, cancellationToken);
        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);

        var resultado = new List<MinhaNotaDto>();
        foreach (var nota in notas)
        {
            var turma = await repository.GetTurmaByIdAsync(nota.TurmaId, cancellationToken);
            if (turma is null || !materias.TryGetValue(turma.MateriaId, out var materia))
            {
                continue;
            }
            resultado.Add(new MinhaNotaDto(nota.Id, nota.TurmaId, materia.Nome, nota.Tipo, nota.Valor));
        }

        return resultado;
    }
}
