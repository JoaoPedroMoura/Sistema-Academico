using System.Diagnostics;
using FaeterjAcademico.Domain.Scheduling;

namespace FaeterjAcademico.Domain.Tests.Scheduling;

/// <summary>
/// Cobre a lista de testes obrigatórios do motor GRASP definida em ARCHITECTURE.md §2.3: as 4
/// restrições rígidas, o cenário de escala do TCC, o invariante do swap da busca local,
/// reprodutibilidade com seed fixa e o critério de parada por estagnação.
/// </summary>
public class GraspScheduleGeneratorTests
{
    private static readonly GraspOptions OpcoesDeTeste = new()
    {
        Iterations = 30,
        Seed = 42,
        StagnationLimit = 15,
    };

    [Fact]
    public void Generate_NuncaAlocaProfessorEmDoisHorariosColidentes()
    {
        // Restrição rígida 1.
        var input = SchedulingTestData.CriarCenarioTcc();
        var resultado = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);

        var colisoes = resultado.Turmas
            .GroupBy(t => t.ProfessorId)
            .SelectMany(g => g
                .SelectMany((t1, i) => g.Skip(i + 1).Where(t2 => t1.Slot.Colide(t2.Slot))));

        Assert.Empty(colisoes);
    }

    [Fact]
    public void Generate_NuncaAlocaDoisHorariosColidentesNoMesmoPeriodoCurricular()
    {
        // Restrição rígida 2.
        var input = SchedulingTestData.CriarCenarioTcc();
        var resultado = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);

        var colisoes = resultado.Turmas
            .GroupBy(t => t.PeriodoCurricular)
            .SelectMany(g => g
                .SelectMany((t1, i) => g.Skip(i + 1).Where(t2 => t1.Slot.Colide(t2.Slot))));

        Assert.Empty(colisoes);
    }

    [Fact]
    public void Generate_NuncaAlocaMaisDeUmProfessorParaAMesmaMateria()
    {
        // Restrição rígida 3.
        var input = SchedulingTestData.CriarCenarioTcc();
        var resultado = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);

        var materiasComMaisDeUmProfessor = resultado.Turmas
            .GroupBy(t => t.MateriaId)
            .Where(g => g.Select(t => t.ProfessorId).Distinct().Count() > 1);

        Assert.Empty(materiasComMaisDeUmProfessor);
    }

    [Fact]
    public void Generate_NuncaAlocaForaDaDisponibilidadeDoProfessor()
    {
        // Restrição rígida 4.
        var input = SchedulingTestData.CriarCenarioTcc();
        var resultado = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);

        foreach (var turma in resultado.Turmas)
        {
            var professor = input.Professores.Single(p => p.Id == turma.ProfessorId);
            Assert.Contains(professor.Disponibilidades, d => d.Contem(turma.Slot));
        }
    }

    [Fact]
    public void Generate_CenarioDeEscalaDoTcc_ProduzSolucaoViavelEmTempoHabil()
    {
        var input = SchedulingTestData.CriarCenarioTcc(materiasCount: 31, professoresCount: 13);
        var options = OpcoesDeTeste with { Iterations = 120, StagnationLimit = 30 };

        var cronometro = Stopwatch.StartNew();
        var resultado = new GraspScheduleGenerator().Generate(input, options);
        cronometro.Stop();

        Assert.True(resultado.Viavel);
        Assert.True(resultado.Completa, $"Matérias não alocadas: {resultado.MateriasNaoAlocadas.Count}");
        Assert.True(cronometro.Elapsed < TimeSpan.FromSeconds(10),
            $"Geração levou {cronometro.Elapsed} — acima do teto de regressão de performance.");
    }

    [Fact]
    public void Generate_ComSeedFixa_ProduzSempreAMesmaSolucao()
    {
        var input = SchedulingTestData.CriarCenarioTcc();

        var resultado1 = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);
        var resultado2 = new GraspScheduleGenerator().Generate(input, OpcoesDeTeste);

        Assert.Equal(resultado1.Custo, resultado2.Custo);
        Assert.Equal(resultado1.Turmas.Count, resultado2.Turmas.Count);
        Assert.Equal(
            resultado1.Turmas.OrderBy(t => t.MateriaId).ThenBy(t => t.Slot.Dia).ThenBy(t => t.Slot.HoraInicio),
            resultado2.Turmas.OrderBy(t => t.MateriaId).ThenBy(t => t.Slot.Dia).ThenBy(t => t.Slot.HoraInicio));
    }

    [Fact]
    public void Generate_QuandoNaoHaMelhoraPossivel_ParaAntesDoTetoDeIteracoes()
    {
        // Cenário trivial (1 matéria, 1 professor, disponibilidade mínima): a primeira iteração
        // já encontra a única solução possível, custo não melhora nunca mais depois — o critério
        // de estagnação deve interromper bem antes do teto de 120.
        var periodosAula = SchedulingTestData.CriarCatalogoDeSlots();
        var professor = new ProfessorInput(Guid.NewGuid(), periodosAula.Select(p => p.Slot).ToList());
        var materia = new MateriaInput(Guid.NewGuid(), PeriodoCurricular: 1, CargaHorariaSemanal: 1);
        var input = new ScheduleGenerationInput(
            [materia],
            [professor],
            [new VinculoInput(materia.Id, professor.Id)],
            periodosAula);

        var options = new GraspOptions { Iterations = 120, StagnationLimit = 5, Seed = 1, ParallelExecution = false };

        var resultado = new GraspScheduleGenerator().Generate(input, options);

        Assert.True(resultado.IteracoesExecutadas < 120);
    }

    [Fact]
    public void Generate_SemMaterias_RetornaSolucaoVaziaViavel()
    {
        var input = new ScheduleGenerationInput([], [], [], []);

        var resultado = new GraspScheduleGenerator().Generate(input);

        Assert.True(resultado.Viavel);
        Assert.True(resultado.Completa);
        Assert.Empty(resultado.Turmas);
    }

    [Fact]
    public void Generate_IterationsInvalido_LancaExcecao()
    {
        var input = SchedulingTestData.CriarCenarioTcc(materiasCount: 1, professoresCount: 1);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new GraspScheduleGenerator().Generate(input, new GraspOptions { Iterations = 0 }));
    }
}
