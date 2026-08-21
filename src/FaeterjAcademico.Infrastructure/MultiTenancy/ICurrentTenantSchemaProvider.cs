namespace FaeterjAcademico.Infrastructure.MultiTenancy;

/// <summary>
/// Abstrai "qual schema Postgres usar agora" — em runtime, resolvido a partir do tenant da
/// requisição (Finbuckle) e usado para montar a connection string do AcademicoDbContext via
/// <c>search_path</c> (ver AcademicoDbContextOptions.Configure, chamado no registro de DI —
/// Fase 6); em design-time (geração de migration) ou em ferramentas de deploy, resolvido para um
/// valor fixo. Ver ARCHITECTURE.md §3.4.
/// </summary>
public interface ICurrentTenantSchemaProvider
{
    string SchemaName { get; }
}

/// <summary>
/// Schema fixo — usado pela design-time factory (geração de migration) e por ferramentas de
/// deploy que aplicam a mesma migration a cada schema de tenant, um de cada vez.
/// </summary>
public class FixedTenantSchemaProvider(string schemaName) : ICurrentTenantSchemaProvider
{
    public string SchemaName { get; } = schemaName;
}
