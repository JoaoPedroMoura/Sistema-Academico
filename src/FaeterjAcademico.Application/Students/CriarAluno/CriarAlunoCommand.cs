namespace FaeterjAcademico.Application.Students.CriarAluno;

public sealed record CriarAlunoCommand(string Nome, string Email, string Matricula, int PeriodoAtual);
