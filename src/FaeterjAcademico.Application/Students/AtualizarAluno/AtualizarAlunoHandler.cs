using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Students.AtualizarAluno;

/// <summary>Edita nome, período e documentos (email e matrícula são identificadores fixos).</summary>
public sealed class AtualizarAlunoHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AtualizarAlunoCommand, AlunoDto>
{
    public async Task<AlunoDto> HandleAsync(AtualizarAlunoCommand request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Aluno não encontrado.");

        aluno.AtualizarDados(request.Nome, request.PeriodoAtual);
        // Campo ausente = mantém o que já está salvo.
        aluno.AtualizarDocumentos(request.DadosPessoais ?? aluno.DadosPessoais, request.Documentos ?? aluno.Documentos);
        if (aluno.DadosPessoais.Cpf is { } cpf
            && await repository.GetAlunoByCpfAsync(cpf, cancellationToken) is { } outro && outro.Id != aluno.Id)
        {
            throw new UseCaseException("Já existe um aluno com este CPF.");
        }

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Aluno.Editar", "Aluno", aluno.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return AlunoDto.FromEntity(aluno);
    }
}
