using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Auth.TrocarSenha;

/// <summary>
/// Troca de senha self-service — usado tanto pelo fluxo obrigatório de primeira senha temporária
/// (<see cref="Domain.Identity.Account.DeveTrocarSenha"/>, ARCHITECTURE.md §7.5) quanto por uma
/// troca voluntária futura. Exige a senha atual (não é reset de Admin/Secretaria — esse caso, se
/// vier a existir, é outro caso de uso, registrado como trabalho futuro).
/// </summary>
public sealed class TrocarSenhaHandler(
    IIdentityRepository repository,
    IPasswordHasher passwordHasher) : IRequestHandler<TrocarSenhaCommand>
{
    private const int TamanhoMinimoSenha = 8;

    public async Task HandleAsync(TrocarSenhaCommand request, CancellationToken cancellationToken = default)
    {
        var account = await repository.FindAccountByIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não encontrada.");

        if (!passwordHasher.Verify(request.SenhaAtual, account.SenhaHash))
        {
            throw new UseCaseException("Senha atual incorreta.");
        }

        if (request.NovaSenha.Length < TamanhoMinimoSenha)
        {
            throw new UseCaseException($"A nova senha precisa ter pelo menos {TamanhoMinimoSenha} caracteres.");
        }

        if (passwordHasher.Verify(request.NovaSenha, account.SenhaHash))
        {
            throw new UseCaseException("A nova senha precisa ser diferente da senha atual.");
        }

        account.AtualizarSenha(passwordHasher.Hash(request.NovaSenha));
        await repository.SaveChangesAsync(cancellationToken);
    }
}
