namespace FaeterjAcademico.Application.Materials.EnviarMaterial;

/// <summary>
/// <paramref name="ArquivoUrl"/> é um link (ex.: já hospedado em Drive/OneDrive/etc.) — este
/// projeto ainda não integra armazenamento de arquivo próprio (S3/Azure Blob); ver
/// ARCHITECTURE.md, trabalho futuro. Nada aqui finge um upload que não existe.
/// </summary>
public sealed record EnviarMaterialCommand(
    Guid AccountId,
    Guid TurmaId,
    string Titulo,
    string? Descricao,
    string ArquivoUrl,
    string ArquivoNomeOriginal,
    long TamanhoBytes);
