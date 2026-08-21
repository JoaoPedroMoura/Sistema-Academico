using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Marks.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Marks.LancarNota;

/// <summary>Caso de uso "Lançamento de notas" do Perfil Professor (ANALISE-TCC.md §2).</summary>
public sealed class LancarNotaHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<LancarNotaCommand, NotaDto>
{
    public async Task<NotaDto> HandleAsync(LancarNotaCommand request, CancellationToken cancellationToken = default)
    {
        var (professor, _) = await ProfessorAuthorization.ResolverProfessorETurmaAsync(
            repository, request.AccountId, request.TurmaId, cancellationToken);

        var aluno = await repository.GetAlunoByIdAsync(request.AlunoId, cancellationToken)
            ?? throw new UseCaseException("Aluno não encontrado.");

        var nota = new Nota(aluno.Id, request.TurmaId, professor.Id, request.Tipo, request.Valor);
        await repository.AddNotaAsync(nota, cancellationToken);

        await repository.AddLogAsync(new LogSistema(
            currentUser.AccountId, "Nota.Lancar", "Nota", nota.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new NotaDto(nota.Id, aluno.Id, aluno.Nome, nota.TurmaId, nota.Tipo, nota.Valor);
    }
}
