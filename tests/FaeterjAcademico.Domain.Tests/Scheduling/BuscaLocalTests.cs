using FaeterjAcademico.Domain.Common;
using FaeterjAcademico.Domain.Scheduling;

namespace FaeterjAcademico.Domain.Tests.Scheduling;

/// <summary>
/// Testa o invariante da busca local isoladamente (ARCHITECTURE.md §2.3, teste 6): o swap nunca
/// piora o custo nem viola restrição rígida. Acessa <see cref="GraspScheduleGenerator.BuscaLocal"/>
/// e <see cref="SolutionCost"/> via InternalsVisibleTo, para não depender da fase de construção
/// (que é não-determinística por natureza) num teste de invariante.
/// </summary>
public class BuscaLocalTests
{
    private static readonly TimeOnly[] Horarios = [new(7, 0), new(7, 50), new(8, 40), new(9, 30), new(10, 20)];

    private static HorarioSlot Slot(int indice) =>
        new(DiaSemana.Segunda, Horarios[indice], Horarios[indice].AddMinutes(50));

    [Fact]
    public void BuscaLocal_ReduzJanelaTrocandoTurmasEntreProfessores()
    {
        var professor1 = new ProfessorInput(Guid.NewGuid(), [Slot(0), Slot(1), Slot(4)]);
        var professor2 = new ProfessorInput(Guid.NewGuid(), [Slot(1), Slot(4)]);

        // Professor 1 leciona às 7h00 e às 10h20 nessa mesma segunda — 1 janela entre elas.
        // Professor 2 leciona só às 7h50 — 0 janela.
        var turmaA = new TurmaAlocada(Guid.NewGuid(), professor1.Id, Guid.NewGuid(), Slot(0), PeriodoCurricular: 1);
        var turmaB = new TurmaAlocada(Guid.NewGuid(), professor1.Id, Guid.NewGuid(), Slot(4), PeriodoCurricular: 1);
        var turmaC = new TurmaAlocada(Guid.NewGuid(), professor2.Id, Guid.NewGuid(), Slot(1), PeriodoCurricular: 2);

        var turmas = new List<TurmaAlocada> { turmaA, turmaB, turmaC };
        var input = new ScheduleGenerationInput(
            Materias: [],
            Professores: [professor1, professor2],
            Vinculos: [],
            PeriodosAula: []);
        var options = new GraspOptions();

        var custoAntes = SolutionCost.Calcular(turmas, options);
        Assert.Equal(1.0, custoAntes); // 1 janela, sem aula isolada (cada matéria só tem 1 aula)

        GraspScheduleGenerator.BuscaLocal(turmas, input, options);

        var custoDepois = SolutionCost.Calcular(turmas, options);
        Assert.True(custoDepois < custoAntes, $"Esperava melhora: antes={custoAntes}, depois={custoDepois}");
        Assert.Equal(0.0, custoDepois); // trocando turmaB <-> turmaC, professor 1 passa a ter aulas consecutivas.

        AssertSemColisao(turmas);
    }

    [Fact]
    public void BuscaLocal_QuandoNaoHaMelhoraPossivel_NuncaPioraOCusto()
    {
        // Só 1 turma — nada para trocar, custo deve permanecer 0.
        var professor = new ProfessorInput(Guid.NewGuid(), [Slot(0)]);
        var turmas = new List<TurmaAlocada>
        {
            new(Guid.NewGuid(), professor.Id, Guid.NewGuid(), Slot(0), PeriodoCurricular: 1),
        };
        var input = new ScheduleGenerationInput([], [professor], [], []);
        var options = new GraspOptions();

        var custoAntes = SolutionCost.Calcular(turmas, options);
        GraspScheduleGenerator.BuscaLocal(turmas, input, options);
        var custoDepois = SolutionCost.Calcular(turmas, options);

        Assert.Equal(custoAntes, custoDepois);
    }

    private static void AssertSemColisao(List<TurmaAlocada> turmas)
    {
        foreach (var grupo in turmas.GroupBy(t => t.ProfessorId))
        {
            var lista = grupo.ToList();
            for (var i = 0; i < lista.Count; i++)
            {
                for (var j = i + 1; j < lista.Count; j++)
                {
                    Assert.False(lista[i].Slot.Colide(lista[j].Slot));
                }
            }
        }
    }
}
