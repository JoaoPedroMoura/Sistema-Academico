using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Schedule.ExcluirGrade;

/// <summary>
/// Não existia no TCC original (ANALISE-TCC.md) — Admin/Secretaria só geravam grade, nunca
/// precisavam descartar uma. Adicionado por pedido do usuário: útil pra descartar um rascunho ou
/// uma grade antiga sem uso.
///
/// Bloqueada se qualquer Turma da grade já tem Nota ou Presença lançada (ARCHITECTURE.md §7.6 —
/// mesmo motivo do bloqueio de exclusão de Professor: sem isso, o Postgres quebraria a FK
/// <c>Notas/Presencas.TurmaId</c>, que é <c>Restrict</c>). Materiais complementares são
/// <c>Cascade</c> e somem junto — são só links, não histórico acadêmico.
/// </summary>
public sealed class ExcluirGradeHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<ExcluirGradeCommand>
{
    public async Task HandleAsync(ExcluirGradeCommand request, CancellationToken cancellationToken = default)
    {
        var grade = await repository.GetGradeByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Grade não encontrada.");

        if (await repository.GradeTemDadosAcademicosLancadosAsync(grade.Id, cancellationToken))
        {
            await repository.AddLogAsync(
                new LogSistema(currentUser.AccountId, "Grade.Excluir", "Grade", grade.Id, sucesso: false,
                    "Bloqueado: grade tem notas ou presenças lançadas em alguma turma."),
                cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);

            throw new UseCaseException(
                "Não é possível excluir: já existem notas ou presenças lançadas em turmas desta grade.");
        }

        repository.RemoveGrade(grade);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Grade.Excluir", "Grade", grade.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
