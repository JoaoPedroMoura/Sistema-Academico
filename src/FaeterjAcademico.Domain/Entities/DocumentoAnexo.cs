using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

public enum TipoDocumento
{
    Cpf = 1,
    Rg = 2,
    CertidaoNascimento = 3,
    ComprovanteResidencia = 4,
    HistoricoEnsinoMedio = 5,
    TituloEleitor = 6,
    Reservista = 7,
    Diploma = 8,
    Outro = 99,
}

/// <summary>
/// Arquivo digitalizado de um documento de aluno ou professor (<see cref="PessoaId"/> é o Id de um
/// ou de outro). O conteúdo fica no próprio banco — sem storage externo no projeto ainda.
/// </summary>
public class DocumentoAnexo : AuditableEntity
{
    public const long TamanhoMaximoBytes = 5 * 1024 * 1024;
    public static readonly IReadOnlySet<string> TiposPermitidos =
        new HashSet<string> { "application/pdf", "image/jpeg", "image/png" };

    public Guid PessoaId { get; private set; }
    public TipoDocumento Tipo { get; private set; }
    public string NomeArquivo { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long TamanhoBytes { get; private set; }
    public byte[] Conteudo { get; private set; } = [];

    private DocumentoAnexo() { } // EF Core

    public DocumentoAnexo(Guid pessoaId, TipoDocumento tipo, string nomeArquivo, string contentType, byte[] conteudo)
    {
        if (conteudo.Length == 0)
        {
            throw new DomainException("Arquivo vazio.");
        }
        if (conteudo.Length > TamanhoMaximoBytes)
        {
            throw new DomainException("Arquivo maior que 5 MB.");
        }
        if (!TiposPermitidos.Contains(contentType))
        {
            throw new DomainException("Envie PDF, JPG ou PNG.");
        }

        PessoaId = pessoaId;
        Tipo = tipo;
        NomeArquivo = Path.GetFileName(nomeArquivo.Trim());
        ContentType = contentType;
        TamanhoBytes = conteudo.Length;
        Conteudo = conteudo;
    }
}
