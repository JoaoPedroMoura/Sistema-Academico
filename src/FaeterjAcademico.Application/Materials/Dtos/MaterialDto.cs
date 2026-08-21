namespace FaeterjAcademico.Application.Materials.Dtos;

public sealed record MaterialDto(
    Guid Id,
    Guid TurmaId,
    string Titulo,
    string? Descricao,
    string ArquivoUrl,
    string ArquivoNomeOriginal,
    long TamanhoBytes,
    DateTime EnviadoEmUtc);

/// <summary>Visão do Aluno — inclui de qual matéria é o material, já que ele vê de várias turmas juntas.</summary>
public sealed record MeuMaterialDto(
    Guid Id,
    Guid TurmaId,
    string MateriaNome,
    string Titulo,
    string? Descricao,
    string ArquivoUrl,
    string ArquivoNomeOriginal,
    long TamanhoBytes,
    DateTime EnviadoEmUtc);
