using FaeterjAcademico.Domain.Common;
using FaeterjAcademico.Domain.Scheduling;

namespace FaeterjAcademico.Domain.Tests.Scheduling;

/// <summary>
/// Gera cenários de teste para o motor GRASP. O cenário "TCC" reproduz a escala descrita em
/// ANALISE-TCC.md §3.2 (31 disciplinas, 13 professores, 6 dias letivos, 7 períodos/dia), com
/// disponibilidade total dos professores — simplificação deliberada para manter os testes de
/// restrição/performance determinísticos e não depender de uma distribuição de disponibilidade
/// realista (isso é testado separadamente na Fase de integração/E2E).
/// </summary>
internal static class SchedulingTestData
{
    // 7 períodos de 50 min, sem intervalo — cada índice é consecutivo ao anterior.
    private static readonly TimeOnly[] HorariosInicio =
    [
        new(7, 0), new(7, 50), new(8, 40), new(9, 30), new(10, 20), new(11, 10), new(12, 0),
    ];

    public static List<PeriodoAulaInput> CriarCatalogoDeSlots()
    {
        var periodos = new List<PeriodoAulaInput>();
        foreach (DiaSemana dia in Enum.GetValues<DiaSemana>())
        {
            foreach (var inicio in HorariosInicio)
            {
                var fim = inicio.AddMinutes(50);
                periodos.Add(new PeriodoAulaInput(Guid.NewGuid(), new HorarioSlot(dia, inicio, fim)));
            }
        }
        return periodos;
    }

    public static ScheduleGenerationInput CriarCenarioTcc(int materiasCount = 31, int professoresCount = 13)
    {
        var periodosAula = CriarCatalogoDeSlots();
        var todosOsSlots = periodosAula.Select(p => p.Slot).ToList();

        var professores = Enumerable.Range(0, professoresCount)
            .Select(_ => new ProfessorInput(Guid.NewGuid(), todosOsSlots))
            .ToList();

        var materias = Enumerable.Range(0, materiasCount)
            .Select(i => new MateriaInput(Guid.NewGuid(), PeriodoCurricular: (i % 5) + 1, CargaHorariaSemanal: 4))
            .ToList();

        var vinculos = materias
            .Select((m, i) => new VinculoInput(m.Id, professores[i % professores.Count].Id))
            .ToList();

        return new ScheduleGenerationInput(materias, professores, vinculos, periodosAula);
    }
}
