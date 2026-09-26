using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;
using FaeterjAcademico.Domain.Common;
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

        // Valida documentos antes de criar a conta, para não sobrar conta órfã por CPF inválido/duplicado.
        var dadosPessoais = (request.DadosPessoais ?? DadosPessoais.Vazio).Normalizar();
        var documentos = (request.Documentos ?? DocumentosAluno.Vazio).Normalizar();
        if (dadosPessoais.Cpf is { } cpf && await academicoRepository.GetAlunoByCpfAsync(cpf, cancellationToken) is not null)
        {
            throw new UseCaseException("Já existe um aluno com este CPF.");
        }

        var matricula = await GeradorMatricula.GerarAsync(academicoRepository, cancellationToken);

        var senhaTemporaria = TemporaryPasswordGenerator.Gerar();
        var account = new Account(request.Nome, emailNormalizado, passwordHasher.Hash(senhaTemporaria), senhaTemporaria: true);
        await identityRepository.AddAccountAsync(account, cancellationToken);
        await identityRepository.AddAccountTenantRoleAsync(
            new AccountTenantRole(account.Id, currentTenant.TenantId, Role.Aluno), cancellationToken);
        await identityRepository.SaveChangesAsync(cancellationToken);

        var aluno = new Aluno(account.Id, request.Nome, emailNormalizado, matricula, request.PeriodoAtual);
        aluno.AtualizarDocumentos(dadosPessoais, documentos);
        await academicoRepository.AddAlunoAsync(aluno, cancellationToken);
        await academicoRepository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Aluno.Matricular", "Aluno", aluno.Id, sucesso: true),
            cancellationToken);
        await academicoRepository.SaveChangesAsync(cancellationToken);

        return new AlunoMatriculadoDto(AlunoDto.FromEntity(aluno), senhaTemporaria);
    }
}
