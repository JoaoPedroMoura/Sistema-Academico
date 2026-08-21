namespace FaeterjAcademico.Domain.Common;

/// <summary>
/// Violação de uma regra de negócio do domínio (ex.: as 4 restrições rígidas do GRASP, exclusão
/// de professor vinculado a matéria). A camada Api mapeia isso para HTTP 422/400 — nunca deve
/// vazar como erro 500.
/// </summary>
public class DomainException(string message) : Exception(message);
