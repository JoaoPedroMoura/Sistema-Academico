using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Tests.Auth;

/// <summary>Fake determinístico — token "gerado" é só um Guid novo, sem JWT de verdade.</summary>
internal sealed class FakeJwtTokenService : IJwtTokenService
{
    public int RefreshTokensCreated { get; private set; }

    public AccessTokenResult CreateAccessToken(Account account, Tenant tenant, Role role) =>
        new($"access-token-for-{account.Id}-{tenant.Slug}-{role}", DateTime.UtcNow.AddMinutes(15));

    public RawRefreshToken CreateRefreshToken()
    {
        RefreshTokensCreated++;
        var raw = $"raw-refresh-{Guid.NewGuid()}";
        return new RawRefreshToken(raw, HashRefreshToken(raw), DateTime.UtcNow.AddDays(7));
    }

    public string HashRefreshToken(string rawToken) => $"hash-of:{rawToken}";
}
