using FaeterjAcademico.Application.Attendance.Dtos;
using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Attendance.ListarMinhasPresencas;

/// <summary>Caso de uso "Consulta de presença/frequência" do Perfil Aluno (ANALISE-TCC.md §2).</summary>
public sealed class ListarMinhasPresencasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMinhasPresencasQuery, IReadOnlyList<MinhaPresencaDto>>
{
    public async Task<IReadOnlyList<MinhaPresencaDto>> HandleAsync(ListarMinhasPresencasQuery request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno desta unidade.");

        var presencas = await repository.GetPresencasByAlunoAsync(aluno.Id, cancellationToken);
        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);

        var resultado = new List<MinhaPresencaDto>();
        foreach (var presenca in presencas)
        {
            var turma = await repository.GetTurmaByIdAsync(presenca.TurmaId, cancellationToken);
            if (turma is null || !materias.TryGetValue(turma.MateriaId, out var materia))
            {
                continue;
            }
            resultado.Add(new MinhaPresencaDto(
                presenca.Id, presenca.TurmaId, materia.Nome, presenca.DataAula, presenca.Presente, presenca.Justificativa));
        }

        return [.. resultado.OrderByDescending(p => p.DataAula)];
    }
}
