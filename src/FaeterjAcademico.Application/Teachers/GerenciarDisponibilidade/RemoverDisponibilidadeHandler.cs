using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;

public sealed class RemoverDisponibilidadeHandler(IAcademicoRepository repository)
    : IRequestHandler<RemoverDisponibilidadeCommand, ProfessorDto>
{
    public async Task<ProfessorDto> HandleAsync(RemoverDisponibilidadeCommand request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new UseCaseException("Professor não encontrado.");

        professor.RemoverDisponibilidade(request.DisponibilidadeId);

        await repository.SaveChangesAsync(cancellationToken);
        return ProfessorDto.FromEntity(professor);
    }
}
