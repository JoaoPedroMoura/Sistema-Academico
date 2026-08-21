using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Identity;

/// <summary>
/// Associação N:N entre <see cref="Account"/> e <see cref="Tenant"/>, carregando o papel — uma
/// conta pode ter papéis diferentes em unidades diferentes (ex. Admin em Petrópolis e Professor
/// em outra unidade). Ver ARCHITECTURE.md §3.2.
/// </summary>
public class AccountTenantRole : Entity
{
    public Guid AccountId { get; private set; }
    public Guid TenantId { get; private set; }
    public Role Role { get; private set; }

    private AccountTenantRole() { } // EF Core

    public AccountTenantRole(Guid accountId, Guid tenantId, Role role)
    {
        AccountId = accountId;
        TenantId = tenantId;
        Role = role;
    }
}
