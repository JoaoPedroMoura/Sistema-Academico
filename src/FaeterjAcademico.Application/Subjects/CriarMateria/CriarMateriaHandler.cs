using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Subjects.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Subjects.CriarMateria;

public sealed class CriarMateriaHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<CriarMateriaCommand, MateriaDto>
{
    public async Task<MateriaDto> HandleAsync(CriarMateriaCommand request, CancellationToken cancellationToken = default)
    {
        var materia = new Materia(request.Nome, request.Periodo, request.CargaHorariaSemanal);
        await repository.AddMateriaAsync(materia, cancellationToken);

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Materia.Adicionar", "Materia", materia.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return MateriaDto.FromEntity(materia);
    }
}
