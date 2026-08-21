using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.TeacherSubjects.RemoverVinculo;

public sealed class RemoverVinculoHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<RemoverVinculoCommand>
{
    public async Task HandleAsync(RemoverVinculoCommand request, CancellationToken cancellationToken = default)
    {
        var vinculo = await repository.GetVinculoAsync(request.MateriaId, request.ProfessorId, cancellationToken)
            ?? throw new UseCaseException("Vínculo não encontrado.");

        repository.RemoveVinculo(vinculo);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "MateriaProfessor.Excluir", "MateriaProfessor", vinculo.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
