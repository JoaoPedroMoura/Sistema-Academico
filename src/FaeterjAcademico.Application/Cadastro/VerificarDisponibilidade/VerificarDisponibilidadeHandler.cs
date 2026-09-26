using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Cadastro.VerificarDisponibilidade;

/// <summary>
/// Pré-checagem para o formulário avisar enquanto o usuário digita. Não substitui a validação
/// dos handlers de criação (que continuam sendo a garantia contra duplicidade).
/// </summary>
public sealed class VerificarDisponibilidadeHandler(IIdentityRepository identityRepository)
    : IRequestHandler<VerificarDisponibilidadeQuery, DisponibilidadeDto>
{
    public async Task<DisponibilidadeDto> HandleAsync(VerificarDisponibilidadeQuery request, CancellationToken cancellationToken = default) =>
        new(await identityRepository.FindAccountByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken) is null);
}
