using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Students.Dtos;

public sealed record AlunoDto(
    Guid Id,
    string Nome,
    string Email,
    string Matricula,
    int PeriodoAtual,
    bool Ativo,
    DadosPessoais DadosPessoais,
    DocumentosAluno Documentos)
{
    public static AlunoDto FromEntity(Domain.Entities.Aluno aluno) =>
        new(aluno.Id, aluno.Nome, aluno.Email, aluno.Matricula, aluno.PeriodoAtual, aluno.Ativo, aluno.DadosPessoais, aluno.Documentos);
}

/// <summary>Só retornado na matrícula — a senha temporária não fica salva em lugar nenhum além do hash.</summary>
public sealed record AlunoMatriculadoDto(AlunoDto Aluno, string SenhaTemporaria);
