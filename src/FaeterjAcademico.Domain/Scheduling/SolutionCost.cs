namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Função objetivo da busca local (ARCHITECTURE.md §2.3) — substitui o "mede tempo" do TCC
/// original por uma métrica de qualidade real, combinando dois proxies da restrição flexível
/// "evitar janelas" (ANALISE-TCC.md §1):
/// <list type="bullet">
/// <item><b>Janelas</b>: gaps no dia de um professor entre uma aula e a próxima — mede o quanto
/// o dia de cada professor está fragmentado.</item>
/// <item><b>Aulas isoladas</b>: dias em que uma matéria com mais de 1 aula/semana aparece sozinha
/// — mede o quanto as aulas de uma mesma matéria estão espalhadas em vez de em blocos.</item>
/// </list>
/// </summary>
internal static class SolutionCost
{
    public static double Calcular(IReadOnlyList<TurmaAlocada> turmas, GraspOptions options)
    {
        var janelas = turmas
            .GroupBy(t => t.ProfessorId)
            .SelectMany(porProfessor => porProfessor.GroupBy(t => t.Slot.Dia))
            .Sum(porDia => ContarJanelasNoDia(porDia.ToList()));

        var aulasIsoladas = turmas
            .GroupBy(t => t.MateriaId)
            .Sum(porMateria => ContarAulasIsoladas(porMateria.ToList()));

        return options.JanelaWeight * janelas + options.AulaIsoladaWeight * aulasIsoladas;
    }

    private static int ContarJanelasNoDia(List<TurmaAlocada> aulasDoDia)
    {
        if (aulasDoDia.Count <= 1)
        {
            return 0;
        }

        var ordenadas = aulasDoDia.OrderBy(t => t.Slot.HoraInicio).ToList();
        var janelas = 0;
        for (var i = 1; i < ordenadas.Count; i++)
        {
            if (!ordenadas[i - 1].Slot.EhConsecutivoA(ordenadas[i].Slot))
            {
                janelas++;
            }
        }
        return janelas;
    }

    private static int ContarAulasIsoladas(List<TurmaAlocada> aulasDaMateria)
    {
        if (aulasDaMateria.Count <= 1)
        {
            return 0; // matéria com só 1 aula/semana não tem como estar "isolada" — é o normal dela.
        }

        return aulasDaMateria
            .GroupBy(t => t.Slot.Dia)
            .Count(porDia => porDia.Count() == 1);
    }
}
