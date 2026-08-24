using FaeterjAcademico.Application.Auth.TrocarSenha;
using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

public class TrocarSenhaHandlerTests
{
    private readonly FakeIdentityRepository _repository = new();
    private readonly FakePasswordHasher _hasher = new();

    private TrocarSenhaHandler CreateHandler() => new(_repository, _hasher);

    private Account AdicionarConta(string senha, bool senhaTemporaria = true)
    {
        var account = new Account("Fulano", "fulano@faeterj.edu.br", _hasher.Hash(senha), senhaTemporaria);
        _repository.Accounts.Add(account);
        return account;
    }

    [Fact]
    public async Task TrocarSenha_SenhaAtualCorretaENovaValida_AtualizaELimpaFlag()
    {
        var account = AdicionarConta("SenhaTemp123");

        await CreateHandler().HandleAsync(new TrocarSenhaCommand(account.Id, "SenhaTemp123", "NovaSenhaForte1"));

        Assert.False(account.DeveTrocarSenha);
        Assert.True(_hasher.Verify("NovaSenhaForte1", account.SenhaHash));
    }

    [Fact]
    public async Task TrocarSenha_SenhaAtualErrada_LancaUseCaseException()
    {
        var account = AdicionarConta("SenhaTemp123");

        await Assert.ThrowsAsync<UseCaseException>(() =>
            CreateHandler().HandleAsync(new TrocarSenhaCommand(account.Id, "SenhaErrada", "NovaSenhaForte1")));

        Assert.True(account.DeveTrocarSenha); // não mudou nada
    }

    [Fact]
    public async Task TrocarSenha_NovaSenhaCurta_LancaUseCaseException()
    {
        var account = AdicionarConta("SenhaTemp123");

        await Assert.ThrowsAsync<UseCaseException>(() =>
            CreateHandler().HandleAsync(new TrocarSenhaCommand(account.Id, "SenhaTemp123", "curta")));
    }

    [Fact]
    public async Task TrocarSenha_NovaSenhaIgualAAtual_LancaUseCaseException()
    {
        var account = AdicionarConta("SenhaTemp123");

        await Assert.ThrowsAsync<UseCaseException>(() =>
            CreateHandler().HandleAsync(new TrocarSenhaCommand(account.Id, "SenhaTemp123", "SenhaTemp123")));
    }

    [Fact]
    public async Task TrocarSenha_ContaInexistente_LancaUseCaseException()
    {
        await Assert.ThrowsAsync<UseCaseException>(() =>
            CreateHandler().HandleAsync(new TrocarSenhaCommand(Guid.NewGuid(), "qualquer1", "outraSenha1")));
    }
}
