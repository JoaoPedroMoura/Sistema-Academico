namespace FaeterjAcademico.Domain.Common;

/// <summary>
/// Base de toda entidade do domínio. Id é gerado no cliente (Guid) para permitir criar o grafo de
/// objetos antes de persistir, sem depender de round-trip ao banco.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
}

/// <summary>
/// Entidade com auditoria de criação/atualização — usada por toda entidade que pode ser alterada
/// depois de criada (quase todas). Ver regra de auditoria preservada em ANALISE-TCC.md §3.3:
/// toda escrita gera LogSistema à parte; estes campos são o "quando", o log é o "o quê/quem".
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }

    protected void Touch() => UpdatedAtUtc = DateTime.UtcNow;
}
