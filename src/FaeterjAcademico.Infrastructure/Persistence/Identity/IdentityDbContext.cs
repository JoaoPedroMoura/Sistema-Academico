using FaeterjAcademico.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity;

/// <summary>
/// Control-plane, schema "identity" — único para toda a aplicação, sem multi-tenancy
/// (ARCHITECTURE.md §3.2).
/// </summary>
public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<AccountTenantRole> AccountTenantRoles => Set<AccountTenantRole>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LoginAudit> LoginAudits => Set<LoginAudit>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly, type =>
            type.Namespace?.Contains(".Persistence.Identity.Configurations") == true);
        base.OnModelCreating(modelBuilder);
    }
}
