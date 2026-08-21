using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.TeacherSubjects.Dtos;

namespace FaeterjAcademico.Application.TeacherSubjects.ListarVinculos;

public sealed class ListarVinculosHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarVinculosQuery, IReadOnlyList<VinculoDto>>
{
    public async Task<IReadOnlyList<VinculoDto>> HandleAsync(ListarVinculosQuery request, CancellationToken cancellationToken = default)
    {
        var vinculos = await repository.GetVinculosAsync(cancellationToken);
        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);
        var professores = (await repository.GetProfessoresAsync(cancellationToken)).ToDictionary(p => p.Id);

        return [.. vinculos
            .Where(v => materias.ContainsKey(v.MateriaId) && professores.ContainsKey(v.ProfessorId))
            .Select(v => new VinculoDto(
                v.Id,
                v.MateriaId,
                materias[v.MateriaId].Nome,
                v.ProfessorId,
                professores[v.ProfessorId].Nome))];
    }
}
