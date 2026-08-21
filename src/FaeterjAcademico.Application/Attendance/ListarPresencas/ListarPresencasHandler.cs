using FaeterjAcademico.Application.Attendance.Dtos;
using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Attendance.ListarPresencas;

public sealed class ListarPresencasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarPresencasQuery, IReadOnlyList<PresencaDto>>
{
    public async Task<IReadOnlyList<PresencaDto>> HandleAsync(ListarPresencasQuery request, CancellationToken cancellationToken = default)
    {
        var presencas = await repository.GetPresencasByTurmaEDataAsync(request.TurmaId, request.DataAula, cancellationToken);
        var alunos = (await repository.GetAlunosAsync(cancellationToken)).ToDictionary(a => a.Id);

        return [.. presencas
            .Where(p => alunos.ContainsKey(p.AlunoId))
            .Select(p => new PresencaDto(p.Id, p.AlunoId, alunos[p.AlunoId].Nome, p.TurmaId, p.DataAula, p.Presente, p.Justificativa))];
    }
}
