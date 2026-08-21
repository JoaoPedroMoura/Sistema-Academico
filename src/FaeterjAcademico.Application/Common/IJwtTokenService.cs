using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Common;

public sealed record AccessTokenResult(string Token, DateTime ExpiresAtUtc);

/// <summary>
/// Token de refresh recém-gerado: valor em claro (devolvido só nesta hora, nunca mais) + seu
/// hash (o que é persistido). Não confundir com o <c>RefreshTokenResult</c> de
/// <c>Application.Auth.Refresh</c> — este aqui é só o material bruto do token; aquele é o
/// resultado completo do caso de uso de refresh (inclui dados de sessão).
/// </summary>
public sealed record RawRefreshToken(string Token, string TokenHash, DateTime ExpiresAtUtc);

/// <summary>Abstrai a emissão de JWT — implementado em Infrastructure (ARCHITECTURE.md §4).</summary>
public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(Account account, Tenant tenant, Role role);
    RawRefreshToken CreateRefreshToken();
    string HashRefreshToken(string rawToken);
}
