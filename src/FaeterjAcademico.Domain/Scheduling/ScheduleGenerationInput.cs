using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Uma disciplina a alocar. Independente de entidade EF — a Application layer mapeia
/// <see cref="Entities.Materia"/> para isto antes de chamar o gerador (Domain não deve conhecer
/// EF Core).
/// </summary>
public sealed record MateriaInput(Guid Id, int PeriodoCurricular, int CargaHorariaSemanal);

/// <summary>Um professor e suas disponibilidades cadastradas.</summary>
public sealed record ProfessorInput(Guid Id, IReadOnlyList<HorarioSlot> Disponibilidades);

/// <summary>Vínculo matéria-professor (quem pode lecionar o quê — ANALISE-TCC.md §2.1).</summary>
public sealed record VinculoInput(Guid MateriaId, Guid ProfessorId);

/// <summary>Um slot discreto do catálogo semanal (o "recurso" que o GRASP aloca).</summary>
public sealed record PeriodoAulaInput(Guid Id, HorarioSlot Slot);

/// <summary>Entrada completa para uma geração de grade.</summary>
public sealed record ScheduleGenerationInput(
    IReadOnlyList<MateriaInput> Materias,
    IReadOnlyList<ProfessorInput> Professores,
    IReadOnlyList<VinculoInput> Vinculos,
    IReadOnlyList<PeriodoAulaInput> PeriodosAula);
