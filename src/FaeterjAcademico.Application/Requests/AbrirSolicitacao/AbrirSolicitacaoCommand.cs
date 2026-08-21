using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Requests.AbrirSolicitacao;

/// <summary><paramref name="AlunoId"/> vem do claim do JWT do aluno autenticado (Fase de área do Aluno).</summary>
public sealed record AbrirSolicitacaoCommand(Guid AlunoId, TipoSolicitacao Tipo, string Descricao, string? AnexoUrl);
