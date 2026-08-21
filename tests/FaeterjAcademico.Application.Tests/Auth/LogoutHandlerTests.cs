using FaeterjAcademico.Application.Auth.Logout;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

public class LogoutHandlerTests
{
    private readonly FakeIdentityRepository _repository = new();
    private readonly FakeJwtTokenService _jwt = new();

    private LogoutHandler CreateHandler() => new(_repository, _jwt);

    [Fact]
    public async Task Logout_TokenValido_Revoga()
    {
        var token = new RefreshToken(Guid.NewGuid(), Guid.NewGuid(), _jwt.HashRefreshToken("raw"), DateTime.UtcNow.AddDays(7));
        _repository.RefreshTokens.Add(token);

        await CreateHandler().HandleAsync(new LogoutCommand("raw"));

        Assert.False(token.Ativo);
    }

    [Fact]
    public async Task Logout_TokenInexistente_NaoLancaExcecao()
    {
        await CreateHandler().HandleAsync(new LogoutCommand("nao-existe"));
        // idempotente — não lança, só não faz nada
    }

    [Fact]
    public async Task Logout_TokenVazio_NaoLancaExcecao()
    {
        await CreateHandler().HandleAsync(new LogoutCommand(""));
    }
}
