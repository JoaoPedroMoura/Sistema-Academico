using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;
using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;

/// <summary>
/// Tela self-service do professor (ANALISE-TCC.md §6 — evolução em relação ao TCC original, onde
/// disponibilidade só era consumida internamente pelo GRASP). Também usável pelo Admin.
/// </summary>
public sealed class AdicionarDisponibilidadeHandler(IAcademicoRepository repository)
    : IRequestHandler<AdicionarDisponibilidadeCommand, ProfessorDto>
{
    public async Task<ProfessorDto> HandleAsync(AdicionarDisponibilidadeCommand request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new UseCaseException("Professor não encontrado.");

        // Professor.AdicionarDisponibilidade já valida sobreposição (DomainException) — deixa
        // propagar, a Api mapeia para 422.
        professor.AdicionarDisponibilidade(new HorarioSlot(request.Dia, request.HoraInicio, request.HoraFim));

        await repository.SaveChangesAsync(cancellationToken);
        return ProfessorDto.FromEntity(professor);
    }
}
