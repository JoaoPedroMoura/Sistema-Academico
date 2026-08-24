using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Teachers.ExcluirProfessor;

/// <summary>
/// Preserva a regra original do TCC (ANALISE-TCC.md §4, UC1/FA03): exclusão bloqueada se o
/// professor estiver vinculado a alguma matéria — inclusive a tentativa bloqueada é logada.
///
/// Também bloqueada se o professor estiver alocado em alguma Turma (de qualquer Grade, não só a
/// ativa) — regra que não existia no TCC original (lá não havia exclusão depois da grade gerada
/// na mesma sessão), mas é necessária aqui: sem ela, o delete quebra a FK
/// <c>Turmas.ProfessorId</c> e vira erro 500 em vez de uma mensagem de negócio.
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
            await BloquearEAsync(professor.Id, "Bloqueado: professor vinculado a uma ou mais matérias.", cancellationToken);
            throw new UseCaseException("Não é possível excluir: professor está vinculado a uma ou mais matérias.");
        }

        if (await repository.ProfessorTemTurmaVinculadaAsync(professor.Id, cancellationToken))
        {
            await BloquearEAsync(professor.Id, "Bloqueado: professor alocado em uma ou mais turmas de alguma grade.", cancellationToken);
            throw new UseCaseException(
                "Não é possível excluir: professor está alocado em uma ou mais turmas de uma grade gerada. " +
                "Gere uma nova grade sem ele antes de excluir.");
        }

        repository.RemoveProfessor(professor);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Professor.Excluir", "Professor", professor.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }

    private async Task BloquearEAsync(Guid professorId, string motivo, CancellationToken cancellationToken)
    {
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Professor.Excluir", "Professor", professorId, sucesso: false, motivo),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
