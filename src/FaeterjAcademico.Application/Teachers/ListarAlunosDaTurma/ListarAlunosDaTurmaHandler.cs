using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.ListarAlunosDaTurma;

/// <summary>Turma de um professor só devolve os alunos da coorte dela — nunca a base inteira de alunos do tenant.</summary>
public sealed class ListarAlunosDaTurmaHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarAlunosDaTurmaQuery, IReadOnlyList<AlunoResumoDto>>
{
    public async Task<IReadOnlyList<AlunoResumoDto>> HandleAsync(ListarAlunosDaTurmaQuery request, CancellationToken cancellationToken = default)
    {
        var (_, turma) = await ProfessorAuthorization.ResolverProfessorETurmaAsync(
            repository, request.AccountId, request.TurmaId, cancellationToken);

        var alunos = await repository.GetAlunosByPeriodoAsync(turma.PeriodoCurricular, cancellationToken);

        return [.. alunos.Select(a => new AlunoResumoDto(a.Id, a.Nome, a.Matricula))];
    }
}
