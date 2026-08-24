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

    /// <summary>
    /// True quando a senha atual é a temporária gerada por Admin/Secretaria ao criar a conta
    /// (<see cref="Application.Common.TemporaryPasswordGenerator"/> — via <c>Application</c>, não
    /// referenciado aqui). O login continua funcionando normalmente com ela; o frontend é quem
    /// força a troca antes de liberar o resto da área (decisão de UX, não de segurança — ver
    /// ARCHITECTURE.md §7.5). Fica <c>false</c> para contas com senha própria, incluindo as
    /// semeadas direto via SQL (Admin/Secretaria iniciais).
    /// </summary>
    public bool DeveTrocarSenha { get; private set; }

    private readonly List<AccountTenantRole> _vinculos = [];
    public IReadOnlyCollection<AccountTenantRole> Vinculos => _vinculos.AsReadOnly();

    private Account() { } // EF Core

    public Account(string nome, string email, string senhaHash, bool senhaTemporaria = false)
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
        DeveTrocarSenha = senhaTemporaria;
    }

    /// <summary>Troca de senha "de verdade" (login/self-service ou reset por Admin) — sempre limpa
    /// <see cref="DeveTrocarSenha"/>, já que a nova senha deixou de ser a temporária.</summary>
    public void AtualizarSenha(string novaSenhaHash)
    {
        if (string.IsNullOrWhiteSpace(novaSenhaHash))
        {
            throw new DomainException("Hash de senha é obrigatório.");
        }
        SenhaHash = novaSenhaHash;
        DeveTrocarSenha = false;
        Touch();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }
}
