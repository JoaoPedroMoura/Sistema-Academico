using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Aluno matriculado na unidade. <see cref="AccountId"/> referencia <c>Identity.Account</c>
/// (mesma observação de <see cref="Professor"/>). Entidade nova nesta evolução — não existia no
/// TCC original (ANALISE-TCC.md §6).
/// </summary>
public class Aluno : AuditableEntity
{
    public Guid AccountId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Matricula { get; private set; } = string.Empty;
    public int PeriodoAtual { get; private set; }
    public bool Ativo { get; private set; } = true;

    private Aluno() { } // EF Core

    public Aluno(Guid accountId, string nome, string email, string matricula, int periodoAtual)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do aluno é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email do aluno é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(matricula))
        {
            throw new DomainException("Matrícula do aluno é obrigatória.");
        }
        if (periodoAtual <= 0)
        {
            throw new DomainException("Período atual do aluno deve ser maior que zero.");
        }

        AccountId = accountId;
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Matricula = matricula.Trim();
        PeriodoAtual = periodoAtual;
    }

    public void AvancarPeriodo(int novoPeriodo)
    {
        if (novoPeriodo <= 0)
        {
            throw new DomainException("Novo período deve ser maior que zero.");
        }
        PeriodoAtual = novoPeriodo;
        Touch();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }
}
