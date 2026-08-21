namespace FaeterjAcademico.Api.Controllers.Materials;

/// <summary>
/// <see cref="ArquivoUrl"/> é um link (armazenamento de arquivo próprio ainda não existe — ver
/// ARCHITECTURE.md, trabalho futuro).
/// </summary>
public sealed record EnviarMaterialRequest(
    Guid TurmaId,
    string Titulo,
    string? Descricao,
    string ArquivoUrl,
    string ArquivoNomeOriginal,
    long TamanhoBytes);
