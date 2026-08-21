using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Material complementar enviado por um professor para os alunos de uma turma
/// (ANALISE-TCC.md §2, Perfil Professor "Upload de materiais" / Perfil Aluno "Download de
/// materiais"). O arquivo em si vive em storage externo; aqui guardamos só a referência.
/// </summary>
public class MaterialComplementar : AuditableEntity
{
    public Guid TurmaId { get; private set; }
    public Guid EnviadoPorProfessorId { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string ArquivoUrl { get; private set; } = string.Empty;
    public string ArquivoNomeOriginal { get; private set; } = string.Empty;
    public long TamanhoBytes { get; private set; }

    private MaterialComplementar() { } // EF Core

    public MaterialComplementar(
        Guid turmaId,
        Guid enviadoPorProfessorId,
        string titulo,
        string arquivoUrl,
        string arquivoNomeOriginal,
        long tamanhoBytes,
        string? descricao = null)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new DomainException("Título do material é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(arquivoUrl))
        {
            throw new DomainException("URL do arquivo é obrigatória.");
        }

        TurmaId = turmaId;
        EnviadoPorProfessorId = enviadoPorProfessorId;
        Titulo = titulo.Trim();
        Descricao = descricao;
        ArquivoUrl = arquivoUrl;
        ArquivoNomeOriginal = arquivoNomeOriginal;
        TamanhoBytes = tamanhoBytes;
    }
}
