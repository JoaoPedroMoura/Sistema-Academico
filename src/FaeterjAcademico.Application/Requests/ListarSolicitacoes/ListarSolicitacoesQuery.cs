using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Requests.ListarSolicitacoes;

public sealed record ListarSolicitacoesQuery(StatusSolicitacao? Status = null);
