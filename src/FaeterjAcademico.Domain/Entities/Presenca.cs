using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Registro de presença/falta de um aluno em uma aula de uma turma, em uma data específica
/// (área Professor: lançamento de presença; área Aluno: consulta de frequência).
/// </summary>
public class Presenca : AuditableEntity
{
    public Guid AlunoId { get; private set; }
    public Guid TurmaId { get; private set; }
    public Guid RegistradaPorProfessorId { get; private set; }
    public DateOnly DataAula { get; private set; }
    public bool Presente { get; private set; }
    public string? Justificativa { get; private set; }

    private Presenca() { } // EF Core

    public Presenca(
        Guid alunoId,
        Guid turmaId,
        Guid registradaPorProfessorId,
        DateOnly dataAula,
        bool presente,
        string? justificativa = null)
    {
        AlunoId = alunoId;
        TurmaId = turmaId;
        RegistradaPorProfessorId = registradaPorProfessorId;
        DataAula = dataAula;
        Presente = presente;
        Justificativa = justificativa;
    }

    /// <summary>Professor corrige um lançamento feito por engano (Perfil Professor).</summary>
    public void Corrigir(bool presente)
    {
        Presente = presente;
        Touch();
    }

    /// <summary>
    /// Aplicada pela Secretaria ao aprovar uma <see cref="Solicitacao"/> de justificativa de
    /// falta (ANALISE-TCC.md §2, Perfil Secretaria).
    /// </summary>
    public void Justificar(string justificativa)
    {
        if (string.IsNullOrWhiteSpace(justificativa))
        {
            throw new DomainException("Justificativa não pode ser vazia.");
        }
        Justificativa = justificativa.Trim();
        Touch();
    }
}
