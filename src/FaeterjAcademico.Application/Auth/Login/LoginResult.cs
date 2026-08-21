using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Auth.Login;

public sealed record TenantOption(string Slug, string Nome, Role Role);

public enum LoginStatus
{
    Sucesso,
    PrecisaEscolherTenant,
}

/// <summary>
/// Resultado do login. Quando a conta tem acesso a mais de uma unidade e nenhum
/// <c>TenantSlug</c> foi informado, <see cref="Status"/> é <see cref="LoginStatus.PrecisaEscolherTenant"/>
/// e nenhum token é emitido — o cliente deve re-chamar o login com o slug escolhido
/// (ARCHITECTURE.md §3.2).
/// </summary>
public sealed record LoginResult
{
    public required LoginStatus Status { get; init; }
    public IReadOnlyList<TenantOption> OpcoesDeTenant { get; init; } = [];

    public string? AccessToken { get; init; }
    public DateTime? AccessTokenExpiresAtUtc { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? RefreshTokenExpiresAtUtc { get; init; }

    public Guid? AccountId { get; init; }
    public string? Nome { get; init; }
    public string? Email { get; init; }
    public string? TenantSlug { get; init; }
    public string? TenantNome { get; init; }
    public Role? Role { get; init; }

    public static LoginResult PrecisaEscolherTenant(IReadOnlyList<TenantOption> opcoes) =>
        new() { Status = LoginStatus.PrecisaEscolherTenant, OpcoesDeTenant = opcoes };

    public static LoginResult Sucesso(
        Account account,
        Tenant tenant,
        Role role,
        string accessToken,
        DateTime accessTokenExpiresAtUtc,
        string refreshToken,
        DateTime refreshTokenExpiresAtUtc) =>
        new()
        {
            Status = LoginStatus.Sucesso,
            AccessToken = accessToken,
            AccessTokenExpiresAtUtc = accessTokenExpiresAtUtc,
            RefreshToken = refreshToken,
            RefreshTokenExpiresAtUtc = refreshTokenExpiresAtUtc,
            AccountId = account.Id,
            Nome = account.Nome,
            Email = account.Email,
            TenantSlug = tenant.Slug,
            TenantNome = tenant.Nome,
            Role = role,
        };
}
