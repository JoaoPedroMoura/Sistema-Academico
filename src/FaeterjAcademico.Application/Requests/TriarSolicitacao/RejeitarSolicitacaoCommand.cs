namespace FaeterjAcademico.Application.Requests.TriarSolicitacao;

public sealed record RejeitarSolicitacaoCommand(Guid SolicitacaoId, string Resposta);
