using FaeterjAcademico.Application.Attendance.Dtos;

namespace FaeterjAcademico.Application.Attendance.RegistrarPresenca;

/// <summary>Lança presença da turma inteira numa data — upsert por aluno (corrige se já lançado).</summary>
public sealed record RegistrarPresencaCommand(
    Guid AccountId,
    Guid TurmaId,
    DateOnly DataAula,
    IReadOnlyList<RegistroPresencaInput> Registros);
