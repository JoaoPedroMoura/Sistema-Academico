using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Checagens rápidas e sem exceção das 4 restrições rígidas (ANALISE-TCC.md §1), usadas no laço
/// quente do GRASP. Espelham as regras já impostas por <see cref="Entities.Grade.AdicionarTurma"/>
/// e <see cref="Entities.Professor.AdicionarDisponibilidade"/> — a versão autoritativa (usada ao
/// persistir a solução vencedora) é a do agregado; esta existe só por performance dentro do laço
/// de milhares de tentativas por geração.
/// </summary>
internal static class SolutionConstraints
{
    /// <summary>Restrição 1: professor não pode ter 2 aulas no mesmo horário.</summary>
    public static bool ColideComProfessor(IReadOnlyList<TurmaAlocada> turmas, Guid professorId, HorarioSlot slot) =>
        turmas.Any(t => t.ProfessorId == professorId && t.Slot.Colide(slot));

    /// <summary>Restrição 2: um período curricular não pode ter 2 aulas no mesmo horário.</summary>
    public static bool ColideComPeriodoCurricular(IReadOnlyList<TurmaAlocada> turmas, int periodoCurricular, HorarioSlot slot) =>
        turmas.Any(t => t.PeriodoCurricular == periodoCurricular && t.Slot.Colide(slot));

    /// <summary>Restrição 4: o slot precisa estar contido em alguma disponibilidade do professor.</summary>
    public static bool DentroDaDisponibilidade(IReadOnlyList<HorarioSlot> disponibilidades, HorarioSlot slot) =>
        disponibilidades.Any(d => d.Contem(slot));

    /// <summary>
    /// Verdadeiro se alocar <paramref name="candidato"/> em <paramref name="slot"/> é viável dado
    /// o estado parcial da solução — as restrições 1, 2 e 4 juntas (a 3 é garantida pelo
    /// desenho da construção: um único professor é escolhido por matéria, nunca dois).
    /// </summary>
    public static bool PodeAlocar(
        IReadOnlyList<TurmaAlocada> turmasAtuais,
        IReadOnlyList<HorarioSlot> disponibilidadesDoProfessor,
        Guid professorId,
        int periodoCurricular,
        HorarioSlot slot) =>
        DentroDaDisponibilidade(disponibilidadesDoProfessor, slot) &&
        !ColideComProfessor(turmasAtuais, professorId, slot) &&
        !ColideComPeriodoCurricular(turmasAtuais, periodoCurricular, slot);
}
