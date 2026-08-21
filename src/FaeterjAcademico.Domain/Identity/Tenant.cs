using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Identity;

/// <summary>
/// Uma unidade da Faeterj (ex. Petrópolis). Vive no schema "identity" (control-plane).
/// <see cref="SchemaName"/> é o nome do schema Postgres com os dados acadêmicos desta unidade —
/// ver ARCHITECTURE.md §3 (schema-por-tenant).
/// </summary>
public class Tenant : AuditableEntity
{
    public string Slug { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;

    public string SchemaName => $"tenant_{Slug}";

    private Tenant() { } // EF Core

    public Tenant(string slug, string nome)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new DomainException("Slug do tenant é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do tenant é obrigatório.");
        }

        Slug = slug.Trim().ToLowerInvariant();
        Nome = nome.Trim();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }
}
