using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Entities;

/// <summary>
/// Professor vinculado à unidade (tenant). <see cref="AccountId"/> referencia
/// <c>Identity.Account</c> no schema "identity" por id — sem FK de banco entre schemas
/// (ARCHITECTURE.md §3), a integridade é garantida pela camada Application.
/// </summary>
public class Professor : AuditableEntity
{
    public Guid AccountId { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Telefone { get; private set; }
    public bool Ativo { get; private set; } = true;

    private readonly List<Disponibilidade> _disponibilidades = [];
    public IReadOnlyCollection<Disponibilidade> Disponibilidades => _disponibilidades.AsReadOnly();

    private Professor() { } // EF Core

    public Professor(Guid accountId, string nome, string email, string? telefone = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do professor é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email do professor é obrigatório.");
        }

        AccountId = accountId;
        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone;
    }

    public void AtualizarDados(string nome, string email, string? telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("Nome do professor é obrigatório.");
        }
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email do professor é obrigatório.");
        }

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        Telefone = telefone;
        Touch();
    }

    public void Desativar() { Ativo = false; Touch(); }
    public void Ativar() { Ativo = true; Touch(); }

    /// <summary>
    /// Regra rígida 4 (ANALISE-TCC.md §1): professor só pode lecionar em horário previamente
    /// cadastrado como disponível. Duplicidade/sobreposição de disponibilidade é rejeitada aqui.
    /// </summary>
    public Disponibilidade AdicionarDisponibilidade(HorarioSlot slot)
    {
        if (_disponibilidades.Any(d => d.Slot.Colide(slot)))
        {
            throw new DomainException(
                $"Já existe disponibilidade cadastrada que colide com {slot.Dia} {slot.HoraInicio}-{slot.HoraFim}.");
        }

        var disponibilidade = new Disponibilidade(Id, slot);
        _disponibilidades.Add(disponibilidade);
        Touch();
        return disponibilidade;
    }

    public void RemoverDisponibilidade(Guid disponibilidadeId)
    {
        _disponibilidades.RemoveAll(d => d.Id == disponibilidadeId);
        Touch();
    }
}
