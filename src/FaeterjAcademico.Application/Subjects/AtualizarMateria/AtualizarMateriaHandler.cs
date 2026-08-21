using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Subjects.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Subjects.AtualizarMateria;

public sealed class AtualizarMateriaHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AtualizarMateriaCommand, MateriaDto>
{
    public async Task<MateriaDto> HandleAsync(AtualizarMateriaCommand request, CancellationToken cancellationToken = default)
    {
        var materia = await repository.GetMateriaByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Matéria não encontrada.");

        materia.AtualizarDados(request.Nome, request.Periodo, request.CargaHorariaSemanal);

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Materia.Editar", "Materia", materia.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MateriaDto.FromEntity(materia);
    }
}
