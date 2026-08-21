namespace FaeterjAcademico.Application.Teachers.CriarProfessor;

public sealed record CriarProfessorCommand(string Nome, string Email, string? Telefone);
