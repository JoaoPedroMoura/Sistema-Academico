using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Log de auditoria de operações de negócio dentro do tenant. Preserva a regra do TCC original
/// (ANALISE-TCC.md §3.3): toda escrita — inclusive tentativas bloqueadas, como excluir professor
/// vinculado a matéria — gera uma entrada aqui. Gravado pela camada Application, nunca pela API
/// diretamente, para garantir que nenhum caso de uso escreva "silenciosamente".
/// </summary>
public class LogSistema : Entity
{
    public Guid? AccountId { get; private set; }
    public string Acao { get; private set; } = string.Empty; // ex.: "Professor.Excluir"
    public string EntidadeTipo { get; private set; } = string.Empty; // ex.: "Professor"
    public Guid? EntidadeId { get; private set; }
    public bool Sucesso { get; private set; }
    public string? Detalhes { get; private set; }
    public DateTime DataHoraUtc { get; private set; } = DateTime.UtcNow;

    private LogSistema() { } // EF Core

    public LogSistema(
        Guid? accountId,
        string acao,
        string entidadeTipo,
        Guid? entidadeId,
        bool sucesso,
        string? detalhes = null)
    {
        if (string.IsNullOrWhiteSpace(acao))
        {
            throw new DomainException("Ação do log é obrigatória.");
        }
        if (string.IsNullOrWhiteSpace(entidadeTipo))
        {
            throw new DomainException("Tipo da entidade do log é obrigatório.");
        }

        AccountId = accountId;
        Acao = acao;
        EntidadeTipo = entidadeTipo;
        EntidadeId = entidadeId;
        Sucesso = sucesso;
        Detalhes = detalhes;
    }
}
