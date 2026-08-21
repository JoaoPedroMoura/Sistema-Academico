using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.ListarProfessores;

public sealed class ListarProfessoresHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarProfessoresQuery, IReadOnlyList<ProfessorDto>>
{
    public async Task<IReadOnlyList<ProfessorDto>> HandleAsync(ListarProfessoresQuery request, CancellationToken cancellationToken = default)
    {
        var professores = await repository.GetProfessoresAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Pesquisa))
        {
            var termo = request.Pesquisa.Trim();
            professores = [.. professores.Where(p =>
                p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                p.Email.Contains(termo, StringComparison.OrdinalIgnoreCase))];
        }

        return [.. professores.Select(ProfessorDto.FromEntity)];
    }
}
