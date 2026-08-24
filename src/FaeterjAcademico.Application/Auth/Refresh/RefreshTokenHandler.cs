using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Auth.Refresh;

/// <summary>
/// Rotação de refresh token: o token usado é sempre revogado (mesmo em caso de reuso indevido —
/// ver checagem abaixo) e um novo par é emitido. Reemite sempre para o mesmo tenant/papel da
/// sessão original (<see cref="Domain.Identity.RefreshToken.TenantId"/>) — trocar de unidade
/// exige um novo login, não um refresh.
/// </summary>
public sealed class RefreshTokenHandler(
    IIdentityRepository repository,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> HandleAsync(RefreshTokenCommand request, CancellationToken cancellationToken = default)
    {
        var hash = jwtTokenService.HashRefreshToken(request.RawRefreshToken);
        var existente = await repository.FindRefreshTokenByHashAsync(hash, cancellationToken);

        if (existente is null || !existente.Ativo)
        {
            throw new AuthenticationFailedException("Sessão expirada — faça login novamente.");
        }

        var conta = await repository.FindAccountByIdAsync(existente.AccountId, cancellationToken)
            ?? throw new AuthenticationFailedException("Conta não encontrada.");

        if (!conta.Ativo)
        {
            throw new AuthenticationFailedException("Conta desativada.");
        }

        var tenant = await repository.FindTenantByIdAsync(existente.TenantId, cancellationToken);
        if (tenant is null || !tenant.Ativo)
        {
            throw new AuthenticationFailedException("Unidade não encontrada ou inativa.");
        }

        var vinculos = await repository.GetRolesAsync(conta.Id, cancellationToken);
        var vinculo = vinculos.FirstOrDefault(v => v.TenantId == tenant.Id)
            ?? throw new AuthenticationFailedException("Esta conta não tem mais acesso a esta unidade.");

        var novoAccessToken = jwtTokenService.CreateAccessToken(conta, tenant, vinculo.Role);
        var novoRefreshToken = jwtTokenService.CreateRefreshToken();

        var novaEntidade = new Domain.Identity.RefreshToken(conta.Id, tenant.Id, novoRefreshToken.TokenHash, novoRefreshToken.ExpiresAtUtc);
        await repository.AddRefreshTokenAsync(novaEntidade, cancellationToken);
        existente.Revogar(novaEntidade.Id);
        await repository.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResult(
            novoAccessToken.Token,
            novoAccessToken.ExpiresAtUtc,
            novoRefreshToken.Token,
            novoRefreshToken.ExpiresAtUtc,
            conta.Id,
            conta.Nome,
            conta.Email,
            tenant.Slug,
            tenant.Nome,
            vinculo.Role,
            conta.DeveTrocarSenha);
    }
}
