using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;
using FaeterjAcademico.Domain.Entities;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Teachers.CriarProfessor;

/// <summary>
/// Cria a conta de login (schema "identity") e o registro de professor (schema do tenant) numa
/// única operação lógica — caso de uso "Manter Professores / Adicionar" do TCC original
/// (ANALISE-TCC.md §4, UC1).
///
/// <b>Consistência entre schemas:</b> Account/AccountTenantRole e Professor vivem em dois
/// <c>DbContext</c> diferentes (identity vs. tenant — ARCHITECTURE.md §3), então não há uma
/// transação de banco única cobrindo os dois <c>SaveChangesAsync</c>. Decisão aceita para o
/// porte deste projeto (evitar `TransactionScope`/2PC): salva a Account primeiro — na pior
/// hipótese de falha depois disso, sobra uma conta órfã sem professor (inofensivo, sem acesso
/// a nada ainda), nunca um professor sem conta de login.
/// </summary>
public sealed class CriarProfessorHandler(
    IIdentityRepository identityRepository,
    IAcademicoRepository academicoRepository,
    IPasswordHasher passwordHasher,
    ICurrentTenantAccessor currentTenant,
    ICurrentUserAccessor currentUser) : IRequestHandler<CriarProfessorCommand, ProfessorCriadoDto>
{
    public async Task<ProfessorCriadoDto> HandleAsync(CriarProfessorCommand request, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        if (await identityRepository.FindAccountByEmailAsync(emailNormalizado, cancellationToken) is not null)
        {
            throw new UseCaseException("Já existe uma conta cadastrada com este email.");
        }

        var senhaTemporaria = TemporaryPasswordGenerator.Gerar();
        var account = new Account(request.Nome, emailNormalizado, passwordHasher.Hash(senhaTemporaria), senhaTemporaria: true);
        await identityRepository.AddAccountAsync(account, cancellationToken);
        await identityRepository.AddAccountTenantRoleAsync(
            new AccountTenantRole(account.Id, currentTenant.TenantId, Role.Professor), cancellationToken);
        await identityRepository.SaveChangesAsync(cancellationToken);

        var professor = new Professor(account.Id, request.Nome, emailNormalizado, request.Telefone);
        await academicoRepository.AddProfessorAsync(professor, cancellationToken);
        await academicoRepository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Professor.Adicionar", "Professor", professor.Id, sucesso: true),
            cancellationToken);
        await academicoRepository.SaveChangesAsync(cancellationToken);

        return new ProfessorCriadoDto(ProfessorDto.FromEntity(professor), senhaTemporaria);
    }
}
