namespace FaeterjAcademico.Application.Requests.Dtos;

public sealed record SolicitacaoDto(
    Guid Id,
    Guid AlunoId,
    string AlunoNome,
    string Tipo,
    string Descricao,
    string? AnexoUrl,
    string Status,
    string? Resposta,
    DateTime AbertaEmUtc,
    DateTime? RespondidaEmUtc);
