using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico;

/// <summary>
/// Configuração compartilhada entre a design-time factory e o registro de DI em runtime
/// (Fase 6) — para não divergir as duas. Isolamento por tenant via <c>search_path</c> da conexão
/// Postgres (não via schema fixo no modelo — ver AcademicoDbContext e ARCHITECTURE.md §3.4).
/// </summary>
public static class AcademicoDbContextOptions
{
    /// <summary>
    /// Garante que o schema do tenant existe no banco. Idempotente. Precisa ser chamado antes de
    /// migrar — <c>search_path</c> aponta para um schema, mas não o cria.
    /// </summary>
    public static void EnsureSchemaExists(string baseConnectionString, string schemaName)
    {
        using var connection = new NpgsqlConnection(baseConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\";";
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Acrescenta <c>search_path</c> à connection string base, para que todo SQL sem schema
    /// explícito (é o caso de toda tabela do AcademicoDbContext) caia no schema do tenant.
    /// </summary>
    public static string WithSearchPath(string baseConnectionString, string schemaName) =>
        $"{baseConnectionString};Options=-c search_path={schemaName}";

    /// <summary>
    /// Só para design-time/deploy (migrations): garante o schema e configura a conexão. Nunca
    /// chamar no caminho de requisição — abre uma conexão extra síncrona a cada chamada.
    /// </summary>
    public static DbContextOptionsBuilder ConfigureForMigration(
        DbContextOptionsBuilder builder,
        string baseConnectionString,
        string schemaName)
    {
        EnsureSchemaExists(baseConnectionString, schemaName);
        return Configure(builder, baseConnectionString, schemaName);
    }

    /// <summary>
    /// Caminho de runtime (registro de DI, Fase 6) — assume que o schema já existe (aplicado no
    /// deploy). Sem side effects: só monta a connection string com <c>search_path</c>. Aceita o
    /// <see cref="DbContextOptionsBuilder"/> não-genérico para funcionar tanto com
    /// <c>AddDbContext</c> (Api) quanto com a design-time factory (genérico).
    /// </summary>
    public static DbContextOptionsBuilder Configure(
        DbContextOptionsBuilder builder,
        string baseConnectionString,
        string schemaName)
    {
        var connectionString = WithSearchPath(baseConnectionString, schemaName);
        builder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schemaName));

        return builder;
    }
}
