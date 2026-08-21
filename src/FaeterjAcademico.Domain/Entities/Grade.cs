using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// A grade de horário gerada para um período letivo. Agregado raiz que garante, ao adicionar
/// cada <see cref="Turma"/>, as restrições rígidas 1-3 do domínio (ANALISE-TCC.md §1):
/// <list type="number">
/// <item>Um professor não pode lecionar mais de uma disciplina no mesmo horário.</item>
/// <item>Uma turma (período curricular) não pode ter mais de uma aula no mesmo horário.</item>
/// <item>Uma disciplina não pode ter mais de um professor alocado simultaneamente na grade.</item>
/// </list>
/// A restrição 4 (disponibilidade do professor) é validada pelo chamador — normalmente o motor
/// GRASP (Fase 5) — antes de chamar <see cref="AdicionarTurma"/>, pois depende de dados de
/// <see cref="Professor"/> que não pertencem a este agregado.
/// </summary>
public class Grade : AuditableEntity
{
    public GradeStatus Status { get; private set; } = GradeStatus.Rascunho;
    public DateTime GeradoEmUtc { get; private set; } = DateTime.UtcNow;
    public double? CustoSolucao { get; private set; }

    private readonly List<Turma> _turmas = [];
    public IReadOnlyCollection<Turma> Turmas => _turmas.AsReadOnly();

    public Turma AdicionarTurma(
        Guid materiaId,
        Guid professorId,
        Guid periodoAulaId,
        HorarioSlot slot,
        int periodoCurricular)
    {
        if (_turmas.Any(t => t.ProfessorId == professorId && t.Slot.Colide(slot)))
        {
            throw new DomainException(
                $"Professor já possui aula alocada em horário que colide com {slot.Dia} {slot.HoraInicio}-{slot.HoraFim} (restrição rígida 1).");
        }

        if (_turmas.Any(t => t.PeriodoCurricular == periodoCurricular && t.Slot.Colide(slot)))
        {
            throw new DomainException(
                $"O {periodoCurricular}º período já possui aula alocada em horário que colide com {slot.Dia} {slot.HoraInicio}-{slot.HoraFim} (restrição rígida 2).");
        }

        if (_turmas.Any(t => t.MateriaId == materiaId && t.ProfessorId != professorId))
        {
            throw new DomainException(
                "Esta disciplina já possui outro professor alocado nesta grade (restrição rígida 3).");
        }

        var turma = new Turma(Id, materiaId, professorId, periodoAulaId, slot, periodoCurricular);
        _turmas.Add(turma);
        Touch();
        return turma;
    }

    public void Publicar(double custoSolucao)
    {
        Status = GradeStatus.Publicada;
        CustoSolucao = custoSolucao;
        Touch();
    }
}
