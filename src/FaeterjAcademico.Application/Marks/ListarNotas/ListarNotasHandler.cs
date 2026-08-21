using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Marks.Dtos;

namespace FaeterjAcademico.Application.Marks.ListarNotas;

public sealed class ListarNotasHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarNotasQuery, IReadOnlyList<NotaDto>>
{
    public async Task<IReadOnlyList<NotaDto>> HandleAsync(ListarNotasQuery request, CancellationToken cancellationToken = default)
    {
        var notas = await repository.GetNotasByTurmaAsync(request.TurmaId, cancellationToken);
        var alunos = (await repository.GetAlunosAsync(cancellationToken)).ToDictionary(a => a.Id);

        return [.. notas
            .Where(n => alunos.ContainsKey(n.AlunoId))
            .Select(n => new NotaDto(n.Id, n.AlunoId, alunos[n.AlunoId].Nome, n.TurmaId, n.Tipo, n.Valor))];
    }
}
