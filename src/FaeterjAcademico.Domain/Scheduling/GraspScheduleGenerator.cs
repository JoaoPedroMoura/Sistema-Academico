namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Motor de geração de grade — port do algoritmo GRASP do TCC (ANALISE-TCC.md §2), com as
/// melhorias de baixo/médio esforço decididas em ARCHITECTURE.md §2.3: RCL com heurística real
/// (mais restrita primeiro), backtracking simples na construção, função objetivo explícita,
/// busca local com swap, critério de parada por estagnação, execução paralela e RNG com seed.
/// </summary>
public sealed class GraspScheduleGenerator : IScheduleGenerator
{
    public ScheduleResult Generate(ScheduleGenerationInput input, GraspOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(input);
        options ??= new GraspOptions();

        if (input.Materias.Count == 0)
        {
            return new ScheduleResult(Viavel: true, Turmas: [], Custo: 0, MateriasNaoAlocadas: [], IteracoesExecutadas: 0);
        }

        if (options.Iterations <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "GraspOptions.Iterations deve ser maior que zero.");
        }

        var baseSeed = options.Seed ?? Environment.TickCount;
        var outcomes = new IterationOutcome?[options.Iterations];

        var batchSize = options.ParallelExecution ? Math.Max(1, Environment.ProcessorCount) : 1;
        var bestIndex = -1;
        var stagnation = 0;
        var executed = 0;

        for (var batchStart = 0; batchStart < options.Iterations; batchStart += batchSize)
        {
            var batchEnd = Math.Min(batchStart + batchSize, options.Iterations);
            RunBatch(input, options, baseSeed, outcomes, batchStart, batchEnd);
            executed = batchEnd;

            var improved = false;
            for (var i = batchStart; i < batchEnd; i++)
            {
                if (IsBetter(outcomes[i]!, bestIndex >= 0 ? outcomes[bestIndex] : null))
                {
                    bestIndex = i;
                    improved = true;
                }
            }

            stagnation = improved ? 0 : stagnation + (batchEnd - batchStart);
            if (stagnation >= options.StagnationLimit)
            {
                break;
            }
        }

        var best = outcomes[bestIndex]!;
        return new ScheduleResult(
            Viavel: true,
            Turmas: best.Turmas,
            Custo: best.Custo,
            MateriasNaoAlocadas: best.MateriasNaoAlocadas,
            IteracoesExecutadas: executed);
    }

    private static void RunBatch(
        ScheduleGenerationInput input,
        GraspOptions options,
        int baseSeed,
        IterationOutcome?[] outcomes,
        int batchStart,
        int batchEnd)
    {
        if (options.ParallelExecution && batchEnd - batchStart > 1)
        {
            Parallel.For(batchStart, batchEnd, i => outcomes[i] = RunIteration(input, options, baseSeed + i));
        }
        else
        {
            for (var i = batchStart; i < batchEnd; i++)
            {
                outcomes[i] = RunIteration(input, options, baseSeed + i);
            }
        }
    }

    /// <summary>Solução completa (sem matéria de fora) sempre vence uma incompleta; empatadas, custo menor vence.</summary>
    private static bool IsBetter(IterationOutcome candidato, IterationOutcome? atual)
    {
        if (atual is null)
        {
            return true;
        }

        var candidatoCompleto = candidato.MateriasNaoAlocadas.Count == 0;
        var atualCompleto = atual.MateriasNaoAlocadas.Count == 0;

        if (candidatoCompleto != atualCompleto)
        {
            return candidatoCompleto;
        }

        if (candidatoCompleto)
        {
            return candidato.Custo < atual.Custo;
        }

        // Nenhuma das duas é completa: prioriza quem alocou mais matérias; empatado, menor custo.
        if (candidato.MateriasNaoAlocadas.Count != atual.MateriasNaoAlocadas.Count)
        {
            return candidato.MateriasNaoAlocadas.Count < atual.MateriasNaoAlocadas.Count;
        }

        return candidato.Custo < atual.Custo;
    }

    private static IterationOutcome RunIteration(ScheduleGenerationInput input, GraspOptions options, int seed)
    {
        var rng = new Random(seed);
        var turmas = new List<TurmaAlocada>();
        var naoAlocadas = new List<Guid>();

        // Heurística "mais restrita primeiro": matérias com menos professores vinculados entram
        // antes na fila, para não sobrarem sem opção quando a disponibilidade já estiver mais
        // ocupada (ARCHITECTURE.md §2.3).
        var pendentes = new Queue<(MateriaInput Materia, int Tentativas)>(
            input.Materias
                .OrderBy(m => input.Vinculos.Count(v => v.MateriaId == m.Id))
                .Select(m => (m, 0)));

        while (pendentes.Count > 0)
        {
            var (materia, tentativas) = pendentes.Dequeue();

            var professoresVinculados = input.Vinculos
                .Where(v => v.MateriaId == materia.Id)
                .Select(v => input.Professores.FirstOrDefault(p => p.Id == v.ProfessorId))
                .Where(p => p is not null)
                .Select(p => p!)
                .ToList();

            var candidatos = professoresVinculados
                .Select(p => (Professor: p, SlotsViaveis: ContarSlotsViaveis(input.PeriodosAula, p, turmas, materia.PeriodoCurricular)))
                .Where(c => c.SlotsViaveis >= materia.CargaHorariaSemanal)
                .OrderBy(c => c.SlotsViaveis) // mais restrito primeiro
                .ToList();

            if (candidatos.Count == 0)
            {
                if (tentativas == 0)
                {
                    pendentes.Enqueue((materia, tentativas + 1));
                }
                else
                {
                    naoAlocadas.Add(materia.Id);
                }
                continue;
            }

            var rclProfessores = Math.Max(1, (int)Math.Ceiling(candidatos.Count * options.Alpha));
            var professorEscolhido = candidatos[rng.Next(Math.Min(rclProfessores, candidatos.Count))].Professor;

            var (sucesso, novasTurmas) = TentarAlocarMateria(materia, professorEscolhido, input, turmas, options, rng);

            if (sucesso)
            {
                turmas.AddRange(novasTurmas);
            }
            else if (tentativas == 0)
            {
                pendentes.Enqueue((materia, tentativas + 1));
            }
            else
            {
                naoAlocadas.Add(materia.Id);
            }
        }

        BuscaLocal(turmas, input, options);

        var custo = SolutionCost.Calcular(turmas, options);
        return new IterationOutcome(turmas, custo, naoAlocadas);
    }

    private static (bool Sucesso, List<TurmaAlocada> Turmas) TentarAlocarMateria(
        MateriaInput materia,
        ProfessorInput professor,
        ScheduleGenerationInput input,
        List<TurmaAlocada> turmasExistentes,
        GraspOptions options,
        Random rng)
    {
        var novasTurmas = new List<TurmaAlocada>();

        for (var i = 0; i < materia.CargaHorariaSemanal; i++)
        {
            var turmasConsideradas = turmasExistentes.Count == 0 && novasTurmas.Count == 0
                ? turmasExistentes
                : [.. turmasExistentes, .. novasTurmas];

            var candidatosSlots = input.PeriodosAula
                .Where(pa => SolutionConstraints.PodeAlocar(
                    turmasConsideradas, professor.Disponibilidades, professor.Id, materia.PeriodoCurricular, pa.Slot))
                .ToList();

            if (candidatosSlots.Count == 0)
            {
                return (false, novasTurmas);
            }

            // Viés guloso: prefere slot consecutivo a uma aula já escolhida desta mesma matéria
            // nesta iteração, para reduzir "janelas" já na construção.
            var ordenados = candidatosSlots
                .OrderByDescending(pa => novasTurmas.Any(t => t.Slot.EhConsecutivoA(pa.Slot)))
                .ToList();

            var rclSlots = Math.Max(1, (int)Math.Ceiling(ordenados.Count * options.Alpha));
            var slotEscolhido = ordenados[rng.Next(Math.Min(rclSlots, ordenados.Count))];

            novasTurmas.Add(new TurmaAlocada(materia.Id, professor.Id, slotEscolhido.Id, slotEscolhido.Slot, materia.PeriodoCurricular));
        }

        return (true, novasTurmas);
    }

    private static int ContarSlotsViaveis(
        IReadOnlyList<PeriodoAulaInput> periodosAula,
        ProfessorInput professor,
        List<TurmaAlocada> turmasAtuais,
        int periodoCurricular) =>
        periodosAula.Count(pa => SolutionConstraints.PodeAlocar(
            turmasAtuais, professor.Disponibilidades, professor.Id, periodoCurricular, pa.Slot));

    /// <summary>
    /// Busca local: hill-climbing por troca de horário entre pares de turmas já alocadas,
    /// aceitando só trocas que reduzem o custo e continuam respeitando as restrições rígidas
    /// (ARCHITECTURE.md §2.3). Primeira melhora encontrada é aplicada; recomeça a varredura até
    /// não haver mais melhora ou até o teto de tentativas.
    /// </summary>
    /// <remarks>internal (não private) para permitir teste isolado do invariante de melhora do swap.</remarks>
    internal static void BuscaLocal(List<TurmaAlocada> turmas, ScheduleGenerationInput input, GraspOptions options)
    {
        if (turmas.Count < 2)
        {
            return;
        }

        var custoAtual = SolutionCost.Calcular(turmas, options);
        var tentativas = 0;
        var melhorou = true;

        while (melhorou && tentativas < options.MaxSwapAttempts)
        {
            melhorou = false;
            for (var i = 0; i < turmas.Count && !melhorou; i++)
            {
                for (var j = i + 1; j < turmas.Count && !melhorou; j++)
                {
                    if (++tentativas > options.MaxSwapAttempts)
                    {
                        return;
                    }

                    if (TentarSwap(turmas, i, j, input, options, ref custoAtual))
                    {
                        melhorou = true;
                    }
                }
            }
        }
    }

    private static bool TentarSwap(
        List<TurmaAlocada> turmas,
        int i,
        int j,
        ScheduleGenerationInput input,
        GraspOptions options,
        ref double custoAtual)
    {
        var a = turmas[i];
        var b = turmas[j];

        if (a.Slot == b.Slot)
        {
            return false;
        }

        var candidatoA = a with { Slot = b.Slot, PeriodoAulaId = b.PeriodoAulaId };
        var candidatoB = b with { Slot = a.Slot, PeriodoAulaId = a.PeriodoAulaId };

        var restante = turmas.Where((_, idx) => idx != i && idx != j).ToList();

        var professorA = input.Professores.FirstOrDefault(p => p.Id == a.ProfessorId);
        var professorB = input.Professores.FirstOrDefault(p => p.Id == b.ProfessorId);
        if (professorA is null || professorB is null)
        {
            return false;
        }

        var restanteComB = new List<TurmaAlocada>(restante) { candidatoB };
        var restanteComA = new List<TurmaAlocada>(restante) { candidatoA };

        var validoA = SolutionConstraints.PodeAlocar(restanteComB, professorA.Disponibilidades, a.ProfessorId, a.PeriodoCurricular, candidatoA.Slot);
        var validoB = SolutionConstraints.PodeAlocar(restanteComA, professorB.Disponibilidades, b.ProfessorId, b.PeriodoCurricular, candidatoB.Slot);

        if (!validoA || !validoB)
        {
            return false;
        }

        var novaLista = new List<TurmaAlocada>(restante) { candidatoA, candidatoB };
        var novoCusto = SolutionCost.Calcular(novaLista, options);

        if (novoCusto >= custoAtual)
        {
            return false;
        }

        turmas[i] = candidatoA;
        turmas[j] = candidatoB;
        custoAtual = novoCusto;
        return true;
    }

    private sealed record IterationOutcome(List<TurmaAlocada> Turmas, double Custo, List<Guid> MateriasNaoAlocadas);
}
