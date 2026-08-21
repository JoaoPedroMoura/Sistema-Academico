namespace FaeterjAcademico.Application.Common;

/// <summary>
/// O tenant resolvido para a requisição atual (Finbuckle, ver ARCHITECTURE.md §3.3) — implementado
/// em Infrastructure. Lança se nenhum tenant foi resolvido (não deveria acontecer em endpoints
/// autenticados, dado o middleware de trava de tenant em Program.cs).
/// </summary>
public interface ICurrentTenantAccessor
{
    Guid TenantId { get; }
    string TenantSlug { get; }
}
