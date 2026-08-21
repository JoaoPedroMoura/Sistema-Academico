using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

public enum TipoSolicitacao
{
    AtestadoMedico = 1,
    RevisaoDeNota = 2,
    JustificativaDeFalta = 3,
    Outro = 4,
}

public enum StatusSolicitacao
{
    Aberta = 1,
    EmAnalise = 2,
    Aprovada = 3,
    Rejeitada = 4,
}

/// <summary>
/// Solicitação aberta por um aluno e triada pela Secretaria (ANALISE-TCC.md §2, Perfil Aluno /
/// Perfil Secretaria). O padrão de auditoria original é preservado por
/// <see cref="Domain.Entities.LogSistema"/>, gravado pela camada Application a cada mudança de
/// status.
/// </summary>
public class Solicitacao : AuditableEntity
{
    public Guid AlunoId { get; private set; }
    public TipoSolicitacao Tipo { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string? AnexoUrl { get; private set; }
    public StatusSolicitacao Status { get; private set; } = StatusSolicitacao.Aberta;
    public Guid? RespondidaPorAccountId { get; private set; }
    public string? Resposta { get; private set; }
    public DateTime? RespondidaEmUtc { get; private set; }

    private Solicitacao() { } // EF Core

    public Solicitacao(Guid alunoId, TipoSolicitacao tipo, string descricao, string? anexoUrl = null)
    {
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new DomainException("Descrição da solicitação é obrigatória.");
        }

        AlunoId = alunoId;
        Tipo = tipo;
        Descricao = descricao.Trim();
        AnexoUrl = anexoUrl;
    }

    public void MarcarEmAnalise()
    {
        GarantirAberta();
        Status = StatusSolicitacao.EmAnalise;
        Touch();
    }

    public void Aprovar(Guid respondidaPorAccountId, string? resposta = null)
    {
        Status = StatusSolicitacao.Aprovada;
        RespondidaPorAccountId = respondidaPorAccountId;
        Resposta = resposta;
        RespondidaEmUtc = DateTime.UtcNow;
        Touch();
    }

    public void Rejeitar(Guid respondidaPorAccountId, string resposta)
    {
        if (string.IsNullOrWhiteSpace(resposta))
        {
            throw new DomainException("É necessário informar o motivo da rejeição.");
        }

        Status = StatusSolicitacao.Rejeitada;
        RespondidaPorAccountId = respondidaPorAccountId;
        Resposta = resposta.Trim();
        RespondidaEmUtc = DateTime.UtcNow;
        Touch();
    }

    private void GarantirAberta()
    {
        if (Status is StatusSolicitacao.Aprovada or StatusSolicitacao.Rejeitada)
        {
            throw new DomainException("Solicitação já foi respondida e não pode ser alterada.");
        }
    }
}
