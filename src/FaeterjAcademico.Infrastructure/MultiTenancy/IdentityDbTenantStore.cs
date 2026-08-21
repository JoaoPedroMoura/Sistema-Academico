using Finbuckle.MultiTenant.Abstractions;
using FaeterjAcademico.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaeterjAcademico.Infrastructure.MultiTenancy;

/// <summary>
/// Store de tenants do Finbuckle apoiada em <see cref="IdentityDbContext.Tenants"/> — não usamos
/// o `EFCoreStoreDbContext` do próprio Finbuckle porque já temos <see cref="Domain.Identity.Tenant"/>
/// como entidade de domínio no schema "identity" (ARCHITECTURE.md §3.2); este store só traduz
/// entre as duas representações.
/// </summary>
public sealed class IdentityDbTenantStore(IdentityDbContext db) : IMultiTenantStore<AppTenantInfo>
{
    public async Task<AppTenantInfo?> GetAsync(string id)
    {
        if (!Guid.TryParse(id, out var tenantId))
        {
            return null;
        }

        var tenant = await db.Tenants.SingleOrDefaultAsync(t => t.Id == tenantId && t.Ativo);
        return tenant is null ? null : ToTenantInfo(tenant);
    }

    public async Task<AppTenantInfo?> GetByIdentifierAsync(string identifier)
    {
        var slug = identifier.Trim().ToLowerInvariant();
        var tenant = await db.Tenants.SingleOrDefaultAsync(t => t.Slug == slug && t.Ativo);
        return tenant is null ? null : ToTenantInfo(tenant);
    }

    public async Task<IEnumerable<AppTenantInfo>> GetAllAsync() =>
        (await db.Tenants.Where(t => t.Ativo).ToListAsync()).Select(ToTenantInfo);

    public async Task<IEnumerable<AppTenantInfo>> GetAllAsync(int pageNumber, int pageSize) =>
        (await db.Tenants.Where(t => t.Ativo)
            .OrderBy(t => t.Nome)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync())
        .Select(ToTenantInfo);

    // Gestão de tenants (criar/editar/desativar unidade) é feita via caso de uso próprio do
    // Admin sobre a entidade Tenant (Fase 7), não por aqui — o Finbuckle só consome, não
    // gerencia o ciclo de vida do tenant.
    public Task<bool> AddAsync(AppTenantInfo tenantInfo) => Task.FromResult(false);
    public Task<bool> UpdateAsync(AppTenantInfo tenantInfo) => Task.FromResult(false);
    public Task<bool> RemoveAsync(string identifier) => Task.FromResult(false);

    private static AppTenantInfo ToTenantInfo(Domain.Identity.Tenant tenant) => new()
    {
        Id = tenant.Id.ToString(),
        Identifier = tenant.Slug,
        Name = tenant.Nome,
        SchemaName = tenant.SchemaName,
    };
}
