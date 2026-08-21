namespace FaeterjAcademico.Application.Schedule.ListarGrades;

public sealed record ListarGradesQuery;

public sealed record GradeResumoDto(Guid Id, string Status, DateTime GeradoEmUtc, double? CustoSolucao, int QuantidadeTurmas);
