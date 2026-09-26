namespace FaeterjAcademico.Domain.Common;

public enum Titulacao
{
    Graduacao = 1,
    Especializacao = 2,
    Mestrado = 3,
    Doutorado = 4,
}

/// <summary>Maior titulação e currículo Lattes do professor. Tudo opcional.</summary>
public sealed record FormacaoProfessor(Titulacao? Titulacao = null, string? LattesUrl = null)
{
    public static readonly FormacaoProfessor Vazio = new();

    public FormacaoProfessor Normalizar()
    {
        var lattes = DadosPessoais.Limpar(LattesUrl);
        if (lattes is not null && !(Uri.TryCreate(lattes, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https"))
        {
            throw new DomainException("Link do Lattes deve ser uma URL (http/https).");
        }
        return new FormacaoProfessor(Titulacao, lattes);
    }
}
