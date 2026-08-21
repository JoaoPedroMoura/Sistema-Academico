namespace FaeterjAcademico.Application.Schedule.Dtos;

public sealed record TurmaDto(
    Guid Id,
    Guid MateriaId,
    string MateriaNome,
    Guid ProfessorId,
    string ProfessorNome,
    string Dia,
    string HoraInicio,
    string HoraFim,
    int PeriodoCurricular);

public sealed record GradeDto(
    Guid Id,
    string Status,
    DateTime GeradoEmUtc,
    double? CustoSolucao,
    IReadOnlyList<TurmaDto> Turmas);

public sealed record GerarGradeResultDto(GradeDto Grade, bool Completa, IReadOnlyList<string> MateriasNaoAlocadas);
