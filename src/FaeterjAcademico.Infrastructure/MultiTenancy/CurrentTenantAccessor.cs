using FaeterjAcademico.Application.Common;
using Finbuckle.MultiTenant.Abstractions;

namespace FaeterjAcademico.Infrastructure.MultiTenancy;

public sealed class CurrentTenantAccessor(IMultiTenantContextAccessor<AppTenantInfo> accessor) : ICurrentTenantAccessor
{
    private AppTenantInfo Tenant =>
        accessor.MultiTenantContext?.TenantInfo
        ?? throw new InvalidOperationException("Nenhum tenant resolvido para a requisição atual.");

    public Guid TenantId => Guid.Parse(Tenant.Id);
    public string TenantSlug => Tenant.Identifier;
}
