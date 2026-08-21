using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Subjects.Dtos;

namespace FaeterjAcademico.Application.Subjects.ListarMaterias;

public sealed class ListarMateriasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMateriasQuery, IReadOnlyList<MateriaDto>>
{
    public async Task<IReadOnlyList<MateriaDto>> HandleAsync(ListarMateriasQuery request, CancellationToken cancellationToken = default)
    {
        var materias = await repository.GetMateriasAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Pesquisa))
        {
            var termo = request.Pesquisa.Trim();
            materias = [.. materias.Where(m => m.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase))];
        }

        return [.. materias.Select(MateriaDto.FromEntity)];
    }
}
