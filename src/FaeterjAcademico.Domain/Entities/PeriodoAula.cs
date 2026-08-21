using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Um slot de horário do catálogo semanal da unidade (ex.: "Segunda, 07h00-07h50, 1º tempo").
/// É o "recurso" que o GRASP aloca a uma <see cref="Turma"/>. Catálogo fixo por tenant — cada
/// unidade pode ter sua própria grade de tempos (ARCHITECTURE.md §3: matérias/grade exclusivas
/// por unidade).
/// </summary>
public class PeriodoAula : Entity
{
    public HorarioSlot Slot { get; private set; } = null!;
    public int Ordem { get; private set; }

    private PeriodoAula() { } // EF Core

    public PeriodoAula(HorarioSlot slot, int ordem)
    {
        if (ordem <= 0)
        {
            throw new DomainException("Ordem do período de aula deve ser maior que zero.");
        }

        Slot = slot;
        Ordem = ordem;
    }
}
