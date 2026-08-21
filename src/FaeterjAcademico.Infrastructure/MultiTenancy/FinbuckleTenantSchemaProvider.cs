using Finbuckle.MultiTenant.Abstractions;

namespace FaeterjAcademico.Infrastructure.MultiTenancy;

/// <summary>
/// Implementação de runtime: lê o schema do tenant resolvido pelo Finbuckle para a requisição
/// atual (subdomínio em produção, header X-Tenant-Slug em dev — ARCHITECTURE.md §3.3).
/// </summary>
public class FinbuckleTenantSchemaProvider(IMultiTenantContextAccessor<AppTenantInfo> accessor)
    : ICurrentTenantSchemaProvider
{
    public string SchemaName =>
        accessor.MultiTenantContext?.TenantInfo?.SchemaName
        ?? throw new InvalidOperationException(
            "Nenhum tenant resolvido para a requisição atual — verifique o header X-Tenant-Slug " +
            "(dev) ou o subdomínio (produção).");
}
