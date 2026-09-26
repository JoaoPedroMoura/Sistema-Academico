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
    public DadosPessoais DadosPessoais { get; private set; } = DadosPessoais.Vazio;
    public DocumentosAluno Documentos { get; private set; } = DocumentosAluno.Vazio;

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

    /// <summary>Email e matrícula não entram aqui: são identificadores fixos do aluno.</summary>
    public void AtualizarDados(string nome, int periodoAtual)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do aluno é obrigatório.");
        }
        if (periodoAtual <= 0)
        {
            throw new DomainException("Período atual do aluno deve ser maior que zero.");
        }

        Nome = nome.Trim();
        PeriodoAtual = periodoAtual;
        Touch();
    }

    /// <summary>Prefixo da matrícula: ano + semestre (1 até 30/06, 2 a partir de 01/07). Ex.: 20261.</summary>
    public static string PrefixoMatricula(DateOnly data) => $"{data.Year}{(data.Month <= 6 ? 1 : 2)}";

    public void AtualizarDocumentos(DadosPessoais dadosPessoais, DocumentosAluno documentos)
    {
        DadosPessoais = dadosPessoais.Normalizar();
        Documentos = documentos.Normalizar();
        Touch();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }
}
