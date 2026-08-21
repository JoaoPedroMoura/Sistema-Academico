using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Api.Controllers.Requests;

public sealed record AbrirSolicitacaoRequest(TipoSolicitacao Tipo, string Descricao, string? AnexoUrl);

public sealed record RejeitarSolicitacaoRequest(string Resposta);

public sealed record AprovarSolicitacaoRequest(string? Resposta);
