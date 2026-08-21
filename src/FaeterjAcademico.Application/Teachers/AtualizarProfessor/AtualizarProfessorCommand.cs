namespace FaeterjAcademico.Application.Teachers.AtualizarProfessor;

public sealed record AtualizarProfessorCommand(Guid Id, string Nome, string Email, string? Telefone);
