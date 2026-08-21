using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.TeacherSubjects.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.TeacherSubjects.AdicionarVinculo;

public sealed class AdicionarVinculoHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AdicionarVinculoCommand, VinculoDto>
{
    public async Task<VinculoDto> HandleAsync(AdicionarVinculoCommand request, CancellationToken cancellationToken = default)
    {
        var materia = await repository.GetMateriaByIdAsync(request.MateriaId, cancellationToken)
            ?? throw new UseCaseException("Matéria não encontrada.");
        var professor = await repository.GetProfessorByIdAsync(request.ProfessorId, cancellationToken)
            ?? throw new UseCaseException("Professor não encontrado.");

        if (await repository.GetVinculoAsync(materia.Id, professor.Id, cancellationToken) is not null)
        {
            throw new UseCaseException("Este professor já está vinculado a esta matéria.");
        }

        var vinculo = new MateriaProfessor(materia.Id, professor.Id);
        await repository.AddVinculoAsync(vinculo, cancellationToken);

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "MateriaProfessor.Adicionar", "MateriaProfessor", vinculo.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new VinculoDto(vinculo.Id, materia.Id, materia.Nome, professor.Id, professor.Nome);
    }
}
