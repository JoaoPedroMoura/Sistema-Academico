using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Auth.Refresh;

public sealed record RefreshTokenResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc,
    Guid AccountId,
    string Nome,
    string Email,
    string TenantSlug,
    string TenantNome,
    Role Role,
    bool PrecisaTrocarSenha);
