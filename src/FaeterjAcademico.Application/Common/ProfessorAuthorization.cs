using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Common;

/// <summary>
/// Resolve o Professor a partir da conta autenticada e confirma que uma Turma é dele — usado por
/// Notas, Presença e Materiais (todas exigem "essa turma é sua?" antes de aceitar a escrita).
/// </summary>
public static class ProfessorAuthorization
{
    public static async Task<(Professor Professor, Turma Turma)> ResolverProfessorETurmaAsync(
        IAcademicoRepository repository, Guid accountId, Guid turmaId, CancellationToken cancellationToken)
    {
        var professor = await repository.GetProfessorByAccountIdAsync(accountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um professor desta unidade.");

        var turma = await repository.GetTurmaByIdAsync(turmaId, cancellationToken)
            ?? throw new UseCaseException("Turma não encontrada.");

        if (turma.ProfessorId != professor.Id)
        {
            throw new UseCaseException("Esta turma não pertence a você.");
        }

        return (professor, turma);
    }
}
