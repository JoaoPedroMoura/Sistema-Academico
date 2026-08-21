using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;

namespace FaeterjAcademico.Application.Students.ListarAlunos;

public sealed class ListarAlunosHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarAlunosQuery, IReadOnlyList<AlunoDto>>
{
    public async Task<IReadOnlyList<AlunoDto>> HandleAsync(ListarAlunosQuery request, CancellationToken cancellationToken = default)
    {
        var alunos = await repository.GetAlunosAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(request.Pesquisa))
        {
            var termo = request.Pesquisa.Trim();
            alunos = [.. alunos.Where(a =>
                a.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                a.Email.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                a.Matricula.Contains(termo, StringComparison.OrdinalIgnoreCase))];
        }

        return [.. alunos.Select(AlunoDto.FromEntity)];
    }
}
