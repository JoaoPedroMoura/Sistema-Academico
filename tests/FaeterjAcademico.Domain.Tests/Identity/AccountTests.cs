using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Domain.Tests.Identity;

public class AccountTests
{
    [Fact]
    public void Construtor_ComSenhaTemporaria_MarcaDeveTrocarSenha()
    {
        var account = new Account("Fulano", "fulano@faeterj.edu.br", "hash", senhaTemporaria: true);

        Assert.True(account.DeveTrocarSenha);
    }

    [Fact]
    public void Construtor_SemSenhaTemporaria_NaoMarcaDeveTrocarSenha()
    {
        var account = new Account("Fulano", "fulano@faeterj.edu.br", "hash");

        Assert.False(account.DeveTrocarSenha);
    }

    [Fact]
    public void AtualizarSenha_LimpaDeveTrocarSenha()
    {
        var account = new Account("Fulano", "fulano@faeterj.edu.br", "hash-temporario", senhaTemporaria: true);

        account.AtualizarSenha("hash-novo");

        Assert.False(account.DeveTrocarSenha);
        Assert.Equal("hash-novo", account.SenhaHash);
    }
}
