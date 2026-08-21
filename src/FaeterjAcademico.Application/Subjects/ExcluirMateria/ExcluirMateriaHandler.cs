using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Subjects.ExcluirMateria;

/// <summary>
/// Preserva a regra original do TCC (ANALISE-TCC.md §4, UC2/FA02): exclusão bloqueada se a
/// matéria estiver vinculada a algum professor.
/// </summary>
public sealed class ExcluirMateriaHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<ExcluirMateriaCommand>
{
    public async Task HandleAsync(ExcluirMateriaCommand request, CancellationToken cancellationToken = default)
    {
        var materia = await repository.GetMateriaByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Matéria não encontrada.");

        if (await repository.MateriaTemVinculoComProfessorAsync(materia.Id, cancellationToken))
        {
            await repository.AddLogAsync(
                new LogSistema(currentUser.AccountId, "Materia.Excluir", "Materia", materia.Id, sucesso: false,
                    "Bloqueado: matéria vinculada a um ou mais professores."),
                cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            throw new UseCaseException("Não é possível excluir: matéria está vinculada a um ou mais professores.");
        }

        repository.RemoveMateria(materia);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Materia.Excluir", "Materia", materia.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
