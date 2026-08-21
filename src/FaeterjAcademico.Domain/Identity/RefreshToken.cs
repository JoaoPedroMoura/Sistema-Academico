using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Identity;

/// <summary>
/// Refresh token rotativo (ARCHITECTURE.md §4). Armazena o hash, nunca o token em claro.
/// <see cref="TenantId"/> fixa a sessão à unidade escolhida no login — refresh sempre reemite
/// para a mesma unidade/papel, nunca troca de tenant silenciosamente (login de novo é exigido
/// para isso).
/// </summary>
public class RefreshToken : Entity
{
    public Guid AccountId { get; private set; }
    public Guid TenantId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CriadoEmUtc { get; private set; } = DateTime.UtcNow;
    public DateTime ExpiraEmUtc { get; private set; }
    public DateTime? RevogadoEmUtc { get; private set; }
    public Guid? SubstituidoPorId { get; private set; }

    public bool Ativo => RevogadoEmUtc is null && DateTime.UtcNow < ExpiraEmUtc;

    private RefreshToken() { } // EF Core

    public RefreshToken(Guid accountId, Guid tenantId, string tokenHash, DateTime expiraEmUtc)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("Hash do refresh token é obrigatório.");
        }

        AccountId = accountId;
        TenantId = tenantId;
        TokenHash = tokenHash;
        ExpiraEmUtc = expiraEmUtc;
    }

    public void Revogar(Guid? substituidoPorId = null)
    {
        RevogadoEmUtc = DateTime.UtcNow;
        SubstituidoPorId = substituidoPorId;
    }
}
