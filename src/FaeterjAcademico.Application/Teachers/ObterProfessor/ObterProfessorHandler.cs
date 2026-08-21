using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.ObterProfessor;

public sealed class ObterProfessorHandler(IAcademicoRepository repository)
    : IRequestHandler<ObterProfessorQuery, ProfessorDto?>
{
    public async Task<ProfessorDto?> HandleAsync(ObterProfessorQuery request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByIdAsync(request.Id, cancellationToken);
        return professor is null ? null : ProfessorDto.FromEntity(professor);
    }
}
