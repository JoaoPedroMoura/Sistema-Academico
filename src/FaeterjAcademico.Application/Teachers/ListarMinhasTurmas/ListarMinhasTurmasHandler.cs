using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.ListarMinhasTurmas;

public sealed class ListarMinhasTurmasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMinhasTurmasQuery, IReadOnlyList<MinhaTurmaDto>>
{
    public async Task<IReadOnlyList<MinhaTurmaDto>> HandleAsync(ListarMinhasTurmasQuery request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um professor desta unidade.");

        var turmas = await repository.GetTurmasByProfessorIdAsync(professor.Id, cancellationToken);
        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);

        return [.. turmas
            .Where(t => materias.ContainsKey(t.MateriaId))
            .OrderBy(t => t.Slot.Dia).ThenBy(t => t.Slot.HoraInicio)
            .Select(t => new MinhaTurmaDto(
                t.Id,
                t.MateriaId,
                materias[t.MateriaId].Nome,
                t.Slot.Dia.ToString(),
                t.Slot.HoraInicio.ToString("HH:mm"),
                t.Slot.HoraFim.ToString("HH:mm"),
                t.PeriodoCurricular))];
    }
}
