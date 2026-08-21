using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Uma aula alocada: liga <see cref="Materia"/> + <see cref="Professor"/> a um
/// <see cref="PeriodoAula"/> (slot) dentro de uma <see cref="Grade"/>, para um período
/// curricular. Só é criada através de <see cref="Grade.AdicionarTurma"/>, que garante as
/// restrições rígidas 1-3 (ANALISE-TCC.md §1) — a restrição 4 (disponibilidade do professor) é
/// checada pelo chamador antes, pois requer conhecer as <see cref="Disponibilidade"/> do
/// professor, que não pertencem ao agregado <see cref="Grade"/>.
/// </summary>
public class Turma : Entity
{
    public Guid GradeId { get; private set; }
    public Guid MateriaId { get; private set; }
    public Guid ProfessorId { get; private set; }
    public Guid PeriodoAulaId { get; private set; }
    public HorarioSlot Slot { get; private set; } = null!;
    public int PeriodoCurricular { get; private set; }

    private Turma() { } // EF Core

    internal Turma(
        Guid gradeId,
        Guid materiaId,
        Guid professorId,
        Guid periodoAulaId,
        HorarioSlot slot,
        int periodoCurricular)
    {
        GradeId = gradeId;
        MateriaId = materiaId;
        ProfessorId = professorId;
        PeriodoAulaId = periodoAulaId;
        Slot = slot;
        PeriodoCurricular = periodoCurricular;
    }
}
