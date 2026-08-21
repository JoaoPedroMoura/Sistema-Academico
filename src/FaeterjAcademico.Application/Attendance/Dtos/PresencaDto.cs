namespace FaeterjAcademico.Application.Attendance.Dtos;

public sealed record PresencaDto(
    Guid Id,
    Guid AlunoId,
    string AlunoNome,
    Guid TurmaId,
    DateOnly DataAula,
    bool Presente,
    string? Justificativa);

public sealed record RegistroPresencaInput(Guid AlunoId, bool Presente);

/// <summary>Visão do Aluno sobre a própria frequência.</summary>
public sealed record MinhaPresencaDto(Guid Id, Guid TurmaId, string MateriaNome, DateOnly DataAula, bool Presente, string? Justificativa);
