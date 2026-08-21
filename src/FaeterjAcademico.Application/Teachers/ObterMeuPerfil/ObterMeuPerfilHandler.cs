using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;

namespace FaeterjAcademico.Application.Teachers.ObterMeuPerfil;

public sealed class ObterMeuPerfilHandler(IAcademicoRepository repository)
    : IRequestHandler<ObterMeuPerfilQuery, ProfessorDto>
{
    public async Task<ProfessorDto> HandleAsync(ObterMeuPerfilQuery request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um professor desta unidade.");

        return ProfessorDto.FromEntity(professor);
    }
}
