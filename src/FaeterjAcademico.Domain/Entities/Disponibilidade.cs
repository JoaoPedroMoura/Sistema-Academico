using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Horário em que um <see cref="Professor"/> está apto a lecionar. No TCC original era só
/// consumida internamente pelo GRASP; aqui vira tela self-service do professor
/// (ANALISE-TCC.md §6). Sempre criada via <see cref="Professor.AdicionarDisponibilidade"/>,
/// que garante ausência de sobreposição.
/// </summary>
public class Disponibilidade : Entity
{
    public Guid ProfessorId { get; private set; }
    public HorarioSlot Slot { get; private set; } = null!;

    private Disponibilidade() { } // EF Core

    internal Disponibilidade(Guid professorId, HorarioSlot slot)
    {
        ProfessorId = professorId;
        Slot = slot;
    }
}
