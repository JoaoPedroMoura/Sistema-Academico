using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Vínculo N:N entre <see cref="Materia"/> e <see cref="Professor"/> — quem pode lecionar o quê.
/// É a fonte de candidatos para a fase de construção do GRASP (ANALISE-TCC.md §2.1, passo 3).
/// Caso de Uso "Manter Matérias do Professor" no TCC original.
/// </summary>
public class MateriaProfessor : Entity
{
    public Guid MateriaId { get; private set; }
    public Guid ProfessorId { get; private set; }

    private MateriaProfessor() { } // EF Core

    public MateriaProfessor(Guid materiaId, Guid professorId)
    {
        MateriaId = materiaId;
        ProfessorId = professorId;
    }
}
