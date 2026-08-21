namespace FaeterjAcademico.Api.Controllers.Marks;

public sealed record LancarNotaRequest(Guid TurmaId, Guid AlunoId, string Tipo, decimal Valor);

public sealed record RevisarNotaRequest(decimal NovoValor);
