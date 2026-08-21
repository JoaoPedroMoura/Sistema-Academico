namespace FaeterjAcademico.Application.Marks.RevisarNota;

public sealed record RevisarNotaCommand(Guid AccountId, Guid NotaId, decimal NovoValor);
