using Finbuckle.MultiTenant.Abstractions;

namespace FaeterjAcademico.Infrastructure.MultiTenancy;

/// <summary>
/// Implementação de <see cref="ITenantInfo"/> exigida pelo Finbuckle. <see cref="Identifier"/> é
/// o slug do tenant (usado na resolução por subdomínio/header — ARCHITECTURE.md §3.3) e
/// <see cref="SchemaName"/> é o schema Postgres com os dados acadêmicos desta unidade.
/// </summary>
public class AppTenantInfo : ITenantInfo
{
    public string Id { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string SchemaName { get; set; } = string.Empty;
}
