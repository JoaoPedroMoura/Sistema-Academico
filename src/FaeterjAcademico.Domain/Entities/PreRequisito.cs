using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Relação de precedência entre matérias: <see cref="MateriaId"/> exige
/// <see cref="MateriaRequisitoId"/> concluída. Não participa das restrições do GRASP (que aloca
/// horário, não matrícula), mas é regra curricular preservada do modelo original.
/// </summary>
public class PreRequisito : Entity
{
    public Guid MateriaId { get; private set; }
    public Guid MateriaRequisitoId { get; private set; }

    private PreRequisito() { } // EF Core

    public PreRequisito(Guid materiaId, Guid materiaRequisitoId)
    {
        if (materiaId == materiaRequisitoId)
        {
            throw new DomainException("Uma matéria não pode ser pré-requisito de si mesma.");
        }

        MateriaId = materiaId;
        MateriaRequisitoId = materiaRequisitoId;
    }
}
