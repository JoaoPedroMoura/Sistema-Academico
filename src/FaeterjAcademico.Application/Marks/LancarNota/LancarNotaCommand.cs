namespace FaeterjAcademico.Application.Marks.LancarNota;

/// <summary><paramref name="AccountId"/> vem do JWT — usado para confirmar que a turma é do professor autenticado.</summary>
public sealed record LancarNotaCommand(Guid AccountId, Guid TurmaId, Guid AlunoId, string Tipo, decimal Valor);
