namespace FaeterjAcademico.Application.Students.AtualizarAluno;

public sealed record AtualizarAlunoCommand(Guid Id, string Nome, int PeriodoAtual);
