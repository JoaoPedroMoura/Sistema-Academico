using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Students.AtualizarAluno;

/// <summary>Edita nome e período (email e matrícula são identificadores fixos).</summary>
public sealed class AtualizarAlunoHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AtualizarAlunoCommand, AlunoDto>
{
    public async Task<AlunoDto> HandleAsync(AtualizarAlunoCommand request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Aluno não encontrado.");

        aluno.AtualizarDados(request.Nome, request.PeriodoAtual);

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Aluno.Editar", "Aluno", aluno.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return AlunoDto.FromEntity(aluno);
    }
}
