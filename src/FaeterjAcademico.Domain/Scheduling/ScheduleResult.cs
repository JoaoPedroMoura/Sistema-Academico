using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Uma aula alocada pelo GRASP. A Application layer converte isto em <see cref="Entities.Turma"/>
/// via <see cref="Entities.Grade.AdicionarTurma"/> ao persistir a solução vencedora.
/// </summary>
public sealed record TurmaAlocada(
    Guid MateriaId,
    Guid ProfessorId,
    Guid PeriodoAulaId,
    HorarioSlot Slot,
    int PeriodoCurricular);

/// <summary>Resultado de uma geração completa (todas as iterações).</summary>
public sealed record ScheduleResult(
    bool Viavel,
    IReadOnlyList<TurmaAlocada> Turmas,
    double Custo,
    IReadOnlyList<Guid> MateriasNaoAlocadas,
    int IteracoesExecutadas)
{
    /// <summary>Viável e sem nenhuma matéria deixada de fora.</summary>
    public bool Completa => Viavel && MateriasNaoAlocadas.Count == 0;
}
