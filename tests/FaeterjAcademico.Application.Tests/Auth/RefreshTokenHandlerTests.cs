using FaeterjAcademico.Application.Auth.Refresh;
using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

public class RefreshTokenHandlerTests
{
    private readonly FakeIdentityRepository _repository = new();
    private readonly FakeJwtTokenService _jwt = new();

    private RefreshTokenHandler CreateHandler() => new(_repository, _jwt);

    private (Account Account, Tenant Tenant) CriarContaComSessao(Role role = Role.Professor)
    {
        var account = new Account("Fulano", "fulano@faeterj.edu.br", "hash");
        var tenant = new Tenant("petropolis", "Faeterj-Petrópolis");
        _repository.Accounts.Add(account);
        _repository.Tenants.Add(tenant);
        _repository.Roles.Add(new AccountTenantRole(account.Id, tenant.Id, role));
        return (account, tenant);
    }

    private RefreshToken CriarRefreshTokenAtivo(Guid accountId, Guid tenantId, string rawToken)
    {
        var token = new RefreshToken(accountId, tenantId, _jwt.HashRefreshToken(rawToken), DateTime.UtcNow.AddDays(7));
        _repository.RefreshTokens.Add(token);
        return token;
    }

    [Fact]
    public async Task Refresh_TokenValido_RotacionaERetornaNovosTokens()
    {
        var (account, tenant) = CriarContaComSessao(Role.Secretaria);
        var original = CriarRefreshTokenAtivo(account.Id, tenant.Id, "raw-original");

        var resultado = await CreateHandler().HandleAsync(new RefreshTokenCommand("raw-original"));

        Assert.NotEqual("raw-original", resultado.RefreshToken);
        Assert.Equal(Role.Secretaria, resultado.Role);
        Assert.Equal("petropolis", resultado.TenantSlug);
        Assert.False(original.Ativo); // token antigo foi revogado (rotação)
        Assert.Equal(2, _repository.RefreshTokens.Count); // o antigo + o novo
    }

    [Fact]
    public async Task Refresh_TokenInexistente_LancaAuthenticationFailedException()
    {
        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new RefreshTokenCommand("token-que-nao-existe")));
    }

    [Fact]
    public async Task Refresh_TokenJaRevogado_LancaAuthenticationFailedException()
    {
        var (account, tenant) = CriarContaComSessao();
        var token = CriarRefreshTokenAtivo(account.Id, tenant.Id, "raw-revogado");
        token.Revogar();

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new RefreshTokenCommand("raw-revogado")));
    }

    [Fact]
    public async Task Refresh_ContaDesativadaAposEmissaoDoToken_LancaAuthenticationFailedException()
    {
        var (account, tenant) = CriarContaComSessao();
        CriarRefreshTokenAtivo(account.Id, tenant.Id, "raw-conta-desativada");
        account.Desativar();

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new RefreshTokenCommand("raw-conta-desativada")));
    }

    [Fact]
    public async Task Refresh_ContaSemMaisAcessoAoTenant_LancaAuthenticationFailedException()
    {
        var (account, tenant) = CriarContaComSessao();
        CriarRefreshTokenAtivo(account.Id, tenant.Id, "raw-sem-acesso");
        _repository.Roles.Clear(); // revoga o vínculo depois do token já ter sido emitido

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new RefreshTokenCommand("raw-sem-acesso")));
    }
}
