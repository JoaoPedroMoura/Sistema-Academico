using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Common;

/// <summary>
/// Acesso ao schema "identity" (control-plane — ARCHITECTURE.md §3.2), usado pelos casos de uso
/// de autenticação. Implementado em Infrastructure sobre o <c>IdentityDbContext</c> — Application
/// não referencia EF Core diretamente.
/// </summary>
public interface IIdentityRepository
{
    Task<Account?> FindAccountByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Account?> FindAccountByIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task AddAccountAsync(Account account, CancellationToken cancellationToken);
    Task AddAccountTenantRoleAsync(AccountTenantRole role, CancellationToken cancellationToken);
    Task<IReadOnlyList<AccountTenantRole>> GetRolesAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Tenant?> FindTenantBySlugAsync(string slug, CancellationToken cancellationToken);
    Task<Tenant?> FindTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken);
    Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task AddLoginAuditAsync(LoginAudit audit, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
