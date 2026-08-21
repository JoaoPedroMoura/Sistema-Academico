using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity;

public sealed class IdentityRepository(IdentityDbContext db) : IIdentityRepository
{
    public Task<Account?> FindAccountByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Accounts.SingleOrDefaultAsync(a => a.Email == email.Trim().ToLower(), cancellationToken);

    public Task<Account?> FindAccountByIdAsync(Guid accountId, CancellationToken cancellationToken) =>
        db.Accounts.SingleOrDefaultAsync(a => a.Id == accountId, cancellationToken);

    public async Task AddAccountAsync(Account account, CancellationToken cancellationToken) =>
        await db.Accounts.AddAsync(account, cancellationToken);

    public async Task AddAccountTenantRoleAsync(AccountTenantRole role, CancellationToken cancellationToken) =>
        await db.AccountTenantRoles.AddAsync(role, cancellationToken);

    public async Task<IReadOnlyList<AccountTenantRole>> GetRolesAsync(Guid accountId, CancellationToken cancellationToken) =>
        await db.AccountTenantRoles.Where(v => v.AccountId == accountId).ToListAsync(cancellationToken);

    public Task<Tenant?> FindTenantBySlugAsync(string slug, CancellationToken cancellationToken) =>
        db.Tenants.SingleOrDefaultAsync(t => t.Slug == slug.Trim().ToLower(), cancellationToken);

    public Task<Tenant?> FindTenantByIdAsync(Guid tenantId, CancellationToken cancellationToken) =>
        db.Tenants.SingleOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken) =>
        await db.RefreshTokens.AddAsync(token, cancellationToken);

    public Task<RefreshToken?> FindRefreshTokenByHashAsync(string tokenHash, CancellationToken cancellationToken) =>
        db.RefreshTokens.SingleOrDefaultAsync(r => r.TokenHash == tokenHash, cancellationToken);

    public async Task AddLoginAuditAsync(LoginAudit audit, CancellationToken cancellationToken) =>
        await db.LoginAudits.AddAsync(audit, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
