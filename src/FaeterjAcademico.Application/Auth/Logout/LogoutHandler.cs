using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Auth.Logout;

/// <summary>Revoga o refresh token — idempotente: token já inválido/inexistente não é erro.</summary>
public sealed class LogoutHandler(IIdentityRepository repository, IJwtTokenService jwtTokenService)
    : IRequestHandler<LogoutCommand>
{
    public async Task HandleAsync(LogoutCommand request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.RawRefreshToken))
        {
            return;
        }

        var hash = jwtTokenService.HashRefreshToken(request.RawRefreshToken);
        var existente = await repository.FindRefreshTokenByHashAsync(hash, cancellationToken);

        if (existente is null || !existente.Ativo)
        {
            return;
        }

        existente.Revogar();
        await repository.SaveChangesAsync(cancellationToken);
    }
}
