using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Schedule.ListarGrades;

public sealed class ListarGradesHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarGradesQuery, IReadOnlyList<GradeResumoDto>>
{
    public async Task<IReadOnlyList<GradeResumoDto>> HandleAsync(ListarGradesQuery request, CancellationToken cancellationToken = default)
    {
        var grades = await repository.GetGradesAsync(cancellationToken);
        return [.. grades.Select(g => new GradeResumoDto(g.Id, g.Status.ToString(), g.GeradoEmUtc, g.CustoSolucao, g.Turmas.Count))];
    }
}
