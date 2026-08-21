using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Nota lançada por um professor para um aluno em uma turma (área Professor, ANALISE-TCC.md §2
/// "Perfil Professor: Lançamento de notas"; consultada pelo aluno na área Aluno).
/// </summary>
public class Nota : AuditableEntity
{
    public Guid AlunoId { get; private set; }
    public Guid TurmaId { get; private set; }
    public Guid LancadaPorProfessorId { get; private set; }
    public string Tipo { get; private set; } = string.Empty; // ex.: "Prova 1", "Trabalho"
    public decimal Valor { get; private set; }

    private Nota() { } // EF Core

    public Nota(Guid alunoId, Guid turmaId, Guid lancadaPorProfessorId, string tipo, decimal valor)
    {
        if (string.IsNullOrWhiteSpace(tipo))
        {
            throw new DomainException("Tipo da nota é obrigatório (ex.: Prova 1, Trabalho).");
        }
        if (valor < 0 || valor > 10)
        {
            throw new DomainException("Valor da nota deve estar entre 0 e 10.");
        }

        AlunoId = alunoId;
        TurmaId = turmaId;
        LancadaPorProfessorId = lancadaPorProfessorId;
        Tipo = tipo.Trim();
        Valor = valor;
    }

    public void Revisar(decimal novoValor)
    {
        if (novoValor < 0 || novoValor > 10)
        {
            throw new DomainException("Valor da nota deve estar entre 0 e 10.");
        }
        Valor = novoValor;
        Touch();
    }
}
