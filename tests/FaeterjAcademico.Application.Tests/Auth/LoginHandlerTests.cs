using FaeterjAcademico.Application.Auth.Login;
using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

public class LoginHandlerTests
{
    private readonly FakeIdentityRepository _repository = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeJwtTokenService _jwt = new();

    private LoginHandler CreateHandler() => new(_repository, _hasher, _jwt);

    private Account AdicionarConta(string email, string senha, bool ativo = true)
    {
        var account = new Account("Fulano", email, _hasher.Hash(senha));
        if (!ativo)
        {
            account.Desativar();
        }
        _repository.Accounts.Add(account);
        return account;
    }

    private Tenant AdicionarTenant(string slug, bool ativo = true)
    {
        var tenant = new Tenant(slug, $"Unidade {slug}");
        if (!ativo)
        {
            tenant.Desativar();
        }
        _repository.Tenants.Add(tenant);
        return tenant;
    }

    private void Vincular(Account account, Tenant tenant, Role role) =>
        _repository.Roles.Add(new AccountTenantRole(account.Id, tenant.Id, role));

    [Fact]
    public async Task Login_CredenciaisValidasEUmSoTenant_RetornaSucessoComTokens()
    {
        var account = AdicionarConta("joao@faeterj.edu.br", "Senha123!");
        var tenant = AdicionarTenant("petropolis");
        Vincular(account, tenant, Role.Admin);

        var resultado = await CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", null, "127.0.0.1"));

        Assert.Equal(LoginStatus.Sucesso, resultado.Status);
        Assert.NotNull(resultado.AccessToken);
        Assert.NotNull(resultado.RefreshToken);
        Assert.Equal(Role.Admin, resultado.Role);
        Assert.Equal("petropolis", resultado.TenantSlug);
        Assert.Single(_repository.RefreshTokens);
        Assert.Contains(_repository.LoginAudits, a => a.Sucesso);
    }

    [Fact]
    public async Task Login_SenhaErrada_LancaAuthenticationFailedEGravaAuditoriaDeFalha()
    {
        AdicionarConta("joao@faeterj.edu.br", "Senha123!");

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "SenhaErrada", null, "127.0.0.1")));

        Assert.Contains(_repository.LoginAudits, a => !a.Sucesso);
        Assert.Empty(_repository.RefreshTokens);
    }

    [Fact]
    public async Task Login_EmailInexistente_LancaAuthenticationFailedException()
    {
        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("ninguem@faeterj.edu.br", "qualquer", null, null)));
    }

    [Fact]
    public async Task Login_ContaDesativada_LancaAuthenticationFailedException()
    {
        AdicionarConta("joao@faeterj.edu.br", "Senha123!", ativo: false);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", null, null)));
    }

    [Fact]
    public async Task Login_ContaSemVinculoComNenhumTenant_LancaAuthenticationFailedException()
    {
        AdicionarConta("joao@faeterj.edu.br", "Senha123!");

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", null, null)));
    }

    [Fact]
    public async Task Login_ComMaisDeUmTenantESemSlugInformado_RetornaPrecisaEscolherTenant()
    {
        var account = AdicionarConta("joao@faeterj.edu.br", "Senha123!");
        var petropolis = AdicionarTenant("petropolis");
        var outra = AdicionarTenant("outraunidade");
        Vincular(account, petropolis, Role.Professor);
        Vincular(account, outra, Role.Aluno);

        var resultado = await CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", null, null));

        Assert.Equal(LoginStatus.PrecisaEscolherTenant, resultado.Status);
        Assert.Equal(2, resultado.OpcoesDeTenant.Count);
        Assert.Null(resultado.AccessToken);
        Assert.Empty(_repository.RefreshTokens); // nenhum token emitido antes da escolha
    }

    [Fact]
    public async Task Login_ComMaisDeUmTenantESlugInformado_LoganaUnidadeEscolhida()
    {
        var account = AdicionarConta("joao@faeterj.edu.br", "Senha123!");
        var petropolis = AdicionarTenant("petropolis");
        var outra = AdicionarTenant("outraunidade");
        Vincular(account, petropolis, Role.Professor);
        Vincular(account, outra, Role.Aluno);

        var resultado = await CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", "outraunidade", null));

        Assert.Equal(LoginStatus.Sucesso, resultado.Status);
        Assert.Equal("outraunidade", resultado.TenantSlug);
        Assert.Equal(Role.Aluno, resultado.Role);
    }

    [Fact]
    public async Task Login_SlugDeTenantQueContaNaoTemAcesso_LancaAuthenticationFailedException()
    {
        var account = AdicionarConta("joao@faeterj.edu.br", "Senha123!");
        var petropolis = AdicionarTenant("petropolis");
        AdicionarTenant("outraunidade");
        Vincular(account, petropolis, Role.Professor);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", "outraunidade", null)));
    }

    [Fact]
    public async Task Login_TenantInativo_LancaAuthenticationFailedException()
    {
        var account = AdicionarConta("joao@faeterj.edu.br", "Senha123!");
        var tenant = AdicionarTenant("petropolis", ativo: false);
        Vincular(account, tenant, Role.Admin);

        await Assert.ThrowsAsync<AuthenticationFailedException>(() =>
            CreateHandler().HandleAsync(new LoginCommand("joao@faeterj.edu.br", "Senha123!", "petropolis", null)));
    }
}
