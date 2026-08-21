using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity;

/// <summary>
/// Usada só por `dotnet ef migrations add/database update` (design-time) — não participa do
/// runtime da aplicação, que resolve a connection string via DI a partir de appsettings/secrets.
/// </summary>
public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("FAETERJ_DB_CONNECTION")
            ?? "Host=localhost;Port=5435;Database=faeterj_academico;Username=faeterj;Password=faeterj_dev_only";

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "identity"))
            .Options;

        return new IdentityDbContext(options);
    }
}
