using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

/// <summary>
/// Fake em memória de <see cref="IIdentityRepository"/> — sem biblioteca de mock, só o
/// suficiente para os testes de LoginHandler/RefreshTokenHandler/LogoutHandler.
/// </summary>
internal sealed class FakeIdentityRepository : IIdentityRepository
{
    public List<Account> Accounts { get; } = [];
    public List<Tenant> Tenants { get; } = [];
    public List<AccountTenantRole> Roles { get; } = [];
    public List<RefreshToken> RefreshTokens { get; } = [];
    public List<LoginAudit> LoginAudits { get; } = [];

    public Task<Account?> FindAccountByEmailAsync(string email, CancellationToken cancellationToken) =>
        Task.FromResult(Accounts.SingleOrDefault(a => a.Email == email.Trim().ToLowerInvariant()));

    public Task<Account?> FindAccountByIdAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult(Accounts.SingleOrDefault(a => a.Id == accountId));

    public Task AddAccountAsync(Account account, CancellationToken cancellationToken)
    {
        Accounts.Add(account);
        return Task.CompletedTask;
    }

    public Task AddAccountTenantRoleAsync(AccountTenantRole role, CancellationToken cancellationToken)
    {
        Roles.Add(role);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AccountTenantRole>> GetRolesAsync(Guid accountId, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<AccountTenantRole>>(Roles.Where(r => r.AccountId == accountId).ToList());

    public Task<Tenant?> FindTenantBySlugAsync(string slug, CancellationToken cancellationToken) =>
        Task.FromResult(Tenants.SingleOrDefault(t => t.Slug == slug.Trim().ToLowerInvariant()));

    public Task<Tenant?> FindTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken) =>
        Task.FromResult(Tenants.SingleOrDefault(t => t.Id == tenantId));

    public Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken)
    {
        RefreshTokens.Add(token);
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        Task.FromResult(RefreshTokens.SingleOrDefault(r => r.TokenHash == tokenHash));

    public Task AddLoginAuditAsync(LoginAudit audit, CancellationToken cancellationToken)
    {
        LoginAudits.Add(audit);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
