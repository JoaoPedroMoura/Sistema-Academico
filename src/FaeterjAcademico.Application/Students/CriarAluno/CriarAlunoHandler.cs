using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;
using FaeterjAcademico.Domain.Entities;
using FaeterjAcademico.Domain.Identity;

namespace FaeterjAcademico.Application.Students.CriarAluno;

/// <summary>
/// Matrícula de aluno — mesma orquestração de <c>CriarProfessorHandler</c> (conta + registro +
/// papel), papel <see cref="Role.Aluno"/>. Responsabilidade da Secretaria: não fazia parte do
/// escopo original do TCC (que não tinha perfil Aluno), mas é pré-requisito para a área existir
/// de verdade — sem aluno matriculado não há quem abra Solicitação para a Secretaria triar.
/// </summary>
public sealed class CriarAlunoHandler(
    IIdentityRepository identityRepository,
    IAcademicoRepository academicoRepository,
    IPasswordHasher passwordHasher,
    ICurrentTenantAccessor currentTenant,
    ICurrentUserAccessor currentUser) : IRequestHandler<CriarAlunoCommand, AlunoMatriculadoDto>
{
    public async Task<AlunoMatriculadoDto> HandleAsync(CriarAlunoCommand request, CancellationToken cancellationToken = default)
    {
        var emailNormalizado = request.Email.Trim().ToLowerInvariant();

        if (await identityRepository.FindAccountByEmailAsync(emailNormalizado, cancellationToken) is not null)
        {
            throw new UseCaseException("Já existe uma conta cadastrada com este email.");
        }

        var matricula = await GerarMatriculaAsync(cancellationToken);

        var senhaTemporaria = TemporaryPasswordGenerator.Gerar();
        var account = new Account(request.Nome, emailNormalizado, passwordHasher.Hash(senhaTemporaria), senhaTemporaria: true);
        await identityRepository.AddAccountAsync(account, cancellationToken);
        await identityRepository.AddAccountTenantRoleAsync(
            new AccountTenantRole(account.Id, currentTenant.TenantId, Role.Aluno), cancellationToken);
        await identityRepository.SaveChangesAsync(cancellationToken);

        var aluno = new Aluno(account.Id, request.Nome, emailNormalizado, matricula, request.PeriodoAtual);
        await academicoRepository.AddAlunoAsync(aluno, cancellationToken);
        await academicoRepository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Aluno.Matricular", "Aluno", aluno.Id, sucesso: true),
            cancellationToken);
        await academicoRepository.SaveChangesAsync(cancellationToken);

        return new AlunoMatriculadoDto(AlunoDto.FromEntity(aluno), senhaTemporaria);
    }

    private static readonly TimeZoneInfo FusoBrasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    /// <summary>Prefixo do semestre (ver <see cref="Aluno.PrefixoMatricula"/>) + 5 dígitos aleatórios, sem repetir.</summary>
    private async Task<string> GerarMatriculaAsync(CancellationToken cancellationToken)
    {
        var hoje = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, FusoBrasilia));
        var prefixo = Aluno.PrefixoMatricula(hoje);

        // ponytail: sorteio com retry; 100k números por semestre, colisão só pesa perto de dezenas de milhares de alunos.
        for (var tentativa = 0; tentativa < 20; tentativa++)
        {
            var matricula = $"{prefixo}{Random.Shared.Next(0, 100_000):D5}";
            if (await academicoRepository.GetAlunoByMatriculaAsync(matricula, cancellationToken) is null)
            {
                return matricula;
            }
        }
        throw new UseCaseException("Não foi possível gerar uma matrícula livre. Tente novamente.");
    }
}
