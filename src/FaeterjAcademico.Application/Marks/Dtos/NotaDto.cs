namespace FaeterjAcademico.Application.Marks.Dtos;

public sealed record NotaDto(Guid Id, Guid AlunoId, string AlunoNome, Guid TurmaId, string Tipo, decimal Valor);

/// <summary>Visão do Aluno sobre a própria nota — troca "quem é o aluno" (óbvio pra ele) por "qual matéria".</summary>
public sealed record MinhaNotaDto(Guid Id, Guid TurmaId, string MateriaNome, string Tipo, decimal Valor);
