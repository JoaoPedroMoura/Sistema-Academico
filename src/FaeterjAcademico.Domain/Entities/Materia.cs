using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Disciplina do curso. <see cref="CargaHorariaSemanal"/> é o número de "tempos" de aula por
/// semana usado como entrada do GRASP (ANALISE-TCC.md §3.2: média de 3,9 tempos/semana no curso
/// original). A gestão de matérias/grade curricular é responsabilidade da Secretaria
/// (ARCHITECTURE.md, evolução vs. TCC original onde era do Admin).
/// </summary>
public class Materia : AuditableEntity
{
    public string Nome { get; private set; } = string.Empty;
    public int Periodo { get; private set; }
    public int CargaHorariaSemanal { get; private set; }
    public bool Ativa { get; private set; } = true;

    private Materia() { } // EF Core

    public Materia(string nome, int periodo, int cargaHorariaSemanal)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome da matéria é obrigatório.");
        }
        if (periodo <= 0)
        {
            throw new DomainException("Período da matéria deve ser maior que zero.");
        }
        if (cargaHorariaSemanal <= 0)
        {
            throw new DomainException("Carga horária semanal deve ser maior que zero.");
        }

        Nome = nome.Trim();
        Periodo = periodo;
        CargaHorariaSemanal = cargaHorariaSemanal;
    }

    public void AtualizarDados(string nome, int periodo, int cargaHorariaSemanal)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome da matéria é obrigatório.");
        }
        if (periodo <= 0)
        {
            throw new DomainException("Período da matéria deve ser maior que zero.");
        }
        if (cargaHorariaSemanal <= 0)
        {
            throw new DomainException("Carga horária semanal deve ser maior que zero.");
        }

        Nome = nome.Trim();
        Periodo = periodo;
        CargaHorariaSemanal = cargaHorariaSemanal;
        Touch();
    }

    public void Desativar() { Ativa = false; Touch(); }
    public void Ativar() { Ativa = true; Touch(); }
}
