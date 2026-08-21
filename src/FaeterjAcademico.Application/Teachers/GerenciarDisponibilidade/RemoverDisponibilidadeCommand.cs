namespace FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;

public sealed record RemoverDisponibilidadeCommand(Guid ProfessorId, Guid DisponibilidadeId);
