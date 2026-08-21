using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Marks.Dtos;

namespace FaeterjAcademico.Application.Marks.RevisarNota;

/// <summary>Caso de uso "Revisão de nota" — pode ser em resposta a uma Solicitacao aprovada pela Secretaria.</summary>
public sealed class RevisarNotaHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<RevisarNotaCommand, NotaDto>
{
    public async Task<NotaDto> HandleAsync(RevisarNotaCommand request, CancellationToken cancellationToken = default)
    {
        var nota = await repository.GetNotaByIdAsync(request.NotaId, cancellationToken)
            ?? throw new UseCaseException("Nota não encontrada.");

        await ProfessorAuthorization.ResolverProfessorETurmaAsync(repository, request.AccountId, nota.TurmaId, cancellationToken);

        nota.Revisar(request.NovoValor);

        await repository.AddLogAsync(new Domain.Entities.LogSistema(
            currentUser.AccountId, "Nota.Revisar", "Nota", nota.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var aluno = await repository.GetAlunoByIdAsync(nota.AlunoId, cancellationToken);
        return new NotaDto(nota.Id, nota.AlunoId, aluno?.Nome ?? string.Empty, nota.TurmaId, nota.Tipo, nota.Valor);
    }
}
