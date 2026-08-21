namespace FaeterjAcademico.Application.Schedule.GerarGrade;

/// <summary><paramref name="Iterations"/> nulo usa o default do motor (120 — ARCHITECTURE.md §2.3).</summary>
public sealed record GerarGradeCommand(int? Iterations = null);
