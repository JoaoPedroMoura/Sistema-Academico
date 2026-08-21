namespace FaeterjAcademico.Application.Common;

/// <summary>
/// A conta autenticada da requisição atual (claim <c>sub</c> do JWT) — implementado em
/// Infrastructure. Usado para preencher <see cref="Domain.Entities.LogSistema.AccountId"/>
/// (ANALISE-TCC.md §3.3: toda escrita gera log de auditoria, inclusive de quem fez).
/// </summary>
public interface ICurrentUserAccessor
{
    Guid? AccountId { get; }
}
