using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico;

/// <summary>
/// Usada por `dotnet ef migrations add/database update` (design-time), e como base para a
/// ferramenta de deploy que aplica a mesma migration a cada schema de tenant existente
/// (ARCHITECTURE.md §3.4). O schema passado aqui só afeta onde a migration é *aplicada* — a
/// migration em si não tem schema gravado (ver AcademicoDbContext), então a mesma migration
/// gerada contra "template" funciona para qualquer tenant.
/// </summary>
public class AcademicoDbContextFactory : IDesignTimeDbContextFactory<AcademicoDbContext>
{
    public const string TemplateSchemaName = "template";

    public AcademicoDbContext CreateDbContext(string[] args)
    {
        var schemaName = args.Length > 0 ? args[0] : TemplateSchemaName;

        var connectionString = Environment.GetEnvironmentVariable("FAETERJ_DB_CONNECTION")
            ?? "Host=localhost;Port=5435;Database=faeterj_academico;Username=faeterj;Password=faeterj_dev_only";

        var builder = new DbContextOptionsBuilder<AcademicoDbContext>();
        AcademicoDbContextOptions.ConfigureForMigration(builder, connectionString, schemaName);

        return new AcademicoDbContext(builder.Options);
    }
}
