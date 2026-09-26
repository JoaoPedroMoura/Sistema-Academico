namespace FaeterjAcademico.Domain.Common;

/// <summary>Documentos exigidos só na matrícula do aluno (ensino médio, título de eleitor, reservista). Tudo opcional.</summary>
public sealed record DocumentosAluno(
    string? EnsinoMedioInstituicao = null,
    int? EnsinoMedioAnoConclusao = null,
    string? TituloEleitor = null,
    string? CertificadoReservista = null)
{
    public static readonly DocumentosAluno Vazio = new();

    public DocumentosAluno Normalizar()
    {
        if (EnsinoMedioAnoConclusao is { } ano && (ano < 1900 || ano > DateTime.UtcNow.Year))
        {
            throw new DomainException("Ano de conclusão do ensino médio inválido.");
        }

        return new DocumentosAluno(
            DadosPessoais.Limpar(EnsinoMedioInstituicao),
            EnsinoMedioAnoConclusao,
            DadosPessoais.Limpar(TituloEleitor),
            DadosPessoais.Limpar(CertificadoReservista));
    }
}
