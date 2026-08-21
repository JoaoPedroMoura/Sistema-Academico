using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Identity;

/// <summary>
/// Conta de login global, independente de tenant (ARCHITECTURE.md §3.2). O vínculo de qual
/// papel a conta tem em qual unidade fica em <see cref="AccountTenantRole"/>.
/// </summary>
public class Account : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;
    public bool Ativo { get; private set; } = true;

    private readonly List<AccountTenantRole> _vinculos = [];
    public IReadOnlyCollection<AccountTenantRole> Vinculos => _vinculos.AsReadOnly();

    private Account() { } // EF Core

    public Account(string nome, string email, string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome da conta é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email da conta é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(senhaHash))
        {
            throw new DomainException("Hash de senha é obrigatório.");
        }

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        SenhaHash = senhaHash;
    }

    public void AtualizarSenha(string novaSenhaHash)
    {
        if (string.IsNullOrWhiteSpace(novaSenhaHash))
        {
            throw new DomainException("Hash de senha é obrigatório.");
        }
        SenhaHash = novaSenhaHash;
        Touch();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }
}
