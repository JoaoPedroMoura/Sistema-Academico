using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Teachers.ExcluirProfessor;

/// <summary>
/// Preserva a regra original do TCC (ANALISE-TCC.md §4, UC1/FA03): exclusão bloqueada se o
/// professor estiver vinculado a alguma matéria — inclusive a tentativa bloqueada é logada.
/// </summary>
public sealed class ExcluirProfessorHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<ExcluirProfessorCommand>
{
    public async Task HandleAsync(ExcluirProfessorCommand request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Professor não encontrado.");

        if (await repository.ProfessorTemVinculoComMateriaAsync(professor.Id, cancellationToken))
        {
            await repository.AddLogAsync(
                new LogSistema(currentUser.AccountId, "Professor.Excluir", "Professor", professor.Id, sucesso: false,
                    "Bloqueado: professor vinculado a uma ou mais matérias."),
                cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            throw new UseCaseException("Não é possível excluir: professor está vinculado a uma ou mais matérias.");
        }

        repository.RemoveProfessor(professor);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Professor.Excluir", "Professor", professor.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
