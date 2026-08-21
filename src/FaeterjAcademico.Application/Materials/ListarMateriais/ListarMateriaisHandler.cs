using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Materials.Dtos;

namespace FaeterjAcademico.Application.Materials.ListarMateriais;

public sealed class ListarMateriaisHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarMateriaisQuery, IReadOnlyList<MaterialDto>>
{
    public async Task<IReadOnlyList<MaterialDto>> HandleAsync(ListarMateriaisQuery request, CancellationToken cancellationToken = default)
    {
        var materiais = await repository.GetMateriaisByTurmaAsync(request.TurmaId, cancellationToken);

        return [.. materiais.Select(m => new MaterialDto(
            m.Id, m.TurmaId, m.Titulo, m.Descricao, m.ArquivoUrl, m.ArquivoNomeOriginal, m.TamanhoBytes, m.CreatedAtUtc))];
    }
}
