using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Identity;

/// <summary>
/// Auditoria de tentativas de login (separada do LogSistema de operações de negócio, que vive
/// por tenant — ver ARCHITECTURE.md §3.2). Registra inclusive tentativas falhas.
/// </summary>
public class LoginAudit : Entity
{
    public string EmailTentativa { get; private set; } = string.Empty;
    public Guid? AccountId { get; private set; }
    public Guid? TenantId { get; private set; }
    public bool Sucesso { get; private set; }
    public string? IpAddress { get; private set; }
    public DateTime DataHoraUtc { get; private set; } = DateTime.UtcNow;

    private LoginAudit() { } // EF Core

    public LoginAudit(string emailTentativa, bool sucesso, Guid? accountId, Guid? tenantId, string? ipAddress)
    {
        EmailTentativa = emailTentativa.Trim().ToLowerInvariant();
        Sucesso = sucesso;
        AccountId = accountId;
        TenantId = tenantId;
        IpAddress = ipAddress;
    }
}
