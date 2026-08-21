using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Materials.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Materials.EnviarMaterial;

public sealed class EnviarMaterialHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<EnviarMaterialCommand, MaterialDto>
{
    public async Task<MaterialDto> HandleAsync(EnviarMaterialCommand request, CancellationToken cancellationToken = default)
    {
        var (professor, _) = await ProfessorAuthorization.ResolverProfessorETurmaAsync(
            repository, request.AccountId, request.TurmaId, cancellationToken);

        var material = new MaterialComplementar(
            request.TurmaId, professor.Id, request.Titulo, request.ArquivoUrl, request.ArquivoNomeOriginal,
            request.TamanhoBytes, request.Descricao);

        await repository.AddMaterialComplementarAsync(material, cancellationToken);
        await repository.AddLogAsync(new LogSistema(
            currentUser.AccountId, "Material.Enviar", "MaterialComplementar", material.Id, sucesso: true), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new MaterialDto(
            material.Id, material.TurmaId, material.Titulo, material.Descricao,
            material.ArquivoUrl, material.ArquivoNomeOriginal, material.TamanhoBytes, material.CreatedAtUtc);
    }
}
