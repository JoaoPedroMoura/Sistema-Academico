namespace FaeterjAcademico.Application.TeacherSubjects.Dtos;

public sealed record VinculoDto(Guid Id, Guid MateriaId, string MateriaNome, Guid ProfessorId, string ProfessorNome);
