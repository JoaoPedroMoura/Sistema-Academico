namespace FaeterjAcademico.Application.Requests.TriarSolicitacao;

public sealed record AprovarSolicitacaoCommand(Guid SolicitacaoId, string? Resposta);
