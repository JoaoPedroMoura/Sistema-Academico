using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Auth.Login;

public sealed class LoginHandler(
    IIdentityRepository repository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> HandleAsync(LoginCommand request, CancellationToken cancellationToken = default)
    {
        var account = await repository.FindAccountByEmailAsync(request.Email, cancellationToken);

        if (account is null || !account.Ativo || !passwordHasher.Verify(request.Password, account.SenhaHash))
        {
            await RegistrarTentativaAsync(request.Email, sucesso: false, account?.Id, tenantId: null, request.IpAddress, cancellationToken);
            throw new AuthenticationFailedException("Email ou senha inválidos.");
        }

        var vinculos = await repository.GetRolesAsync(account.Id, cancellationToken);
        if (vinculos.Count == 0)
        {
            await RegistrarTentativaAsync(request.Email, sucesso: false, account.Id, tenantId: null, request.IpAddress, cancellationToken);
            throw new AuthenticationFailedException("Esta conta não tem acesso a nenhuma unidade.");
        }

        AccountTenantRole vinculoEscolhido;

        if (!string.IsNullOrWhiteSpace(request.TenantSlug))
        {
            var tenantInformado = await repository.FindTenantBySlugAsync(request.TenantSlug, cancellationToken)
                ?? throw new AuthenticationFailedException("Unidade não encontrada.");

            vinculoEscolhido = vinculos.FirstOrDefault(v => v.TenantId == tenantInformado.Id)
                ?? throw new AuthenticationFailedException("Esta conta não tem acesso a esta unidade.");
        }
        else if (vinculos.Count == 1)
        {
            vinculoEscolhido = vinculos[0];
        }
        else
        {
            var opcoes = new List<TenantOption>();
            foreach (var vinculo in vinculos)
            {
                var tenantDoVinculo = await repository.FindTenantByIdAsync(vinculo.TenantId, cancellationToken);
                if (tenantDoVinculo is { Ativo: true })
                {
                    opcoes.Add(new TenantOption(tenantDoVinculo.Slug, tenantDoVinculo.Nome, vinculo.Role));
                }
            }
            return LoginResult.PrecisaEscolherTenant(opcoes);
        }

        var tenant = await repository.FindTenantByIdAsync(vinculoEscolhido.TenantId, cancellationToken)
            ?? throw new AuthenticationFailedException("Unidade não encontrada ou inativa.");

        if (!tenant.Ativo)
        {
            throw new AuthenticationFailedException("Esta unidade está inativa.");
        }

        var accessToken = jwtTokenService.CreateAccessToken(account, tenant, vinculoEscolhido.Role);
        var refreshToken = jwtTokenService.CreateRefreshToken();

        await repository.AddRefreshTokenAsync(
            new Domain.Identity.RefreshToken(account.Id, tenant.Id, refreshToken.TokenHash, refreshToken.ExpiresAtUtc),
            cancellationToken);

        await RegistrarTentativaAsync(request.Email, sucesso: true, account.Id, tenant.Id, request.IpAddress, cancellationToken);

        return LoginResult.Sucesso(
            account,
            tenant,
            vinculoEscolhido.Role,
            accessToken.Token,
            accessToken.ExpiresAtUtc,
            refreshToken.Token,
            refreshToken.ExpiresAtUtc);
    }

    private async Task RegistrarTentativaAsync(
        string email, bool sucesso, Guid? accountId, Guid? tenantId, string? ip, CancellationToken cancellationToken)
    {
        await repository.AddLoginAuditAsync(new LoginAudit(email, sucesso, accountId, tenantId, ip), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
