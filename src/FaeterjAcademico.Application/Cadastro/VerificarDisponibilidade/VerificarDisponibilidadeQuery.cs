namespace FaeterjAcademico.Application.Cadastro.VerificarDisponibilidade;

public sealed record VerificarDisponibilidadeQuery(string Email);

public sealed record DisponibilidadeDto(bool EmailDisponivel);
