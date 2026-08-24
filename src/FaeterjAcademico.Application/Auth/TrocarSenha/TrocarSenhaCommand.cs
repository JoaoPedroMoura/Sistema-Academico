namespace FaeterjAcademico.Application.Auth.TrocarSenha;

public sealed record TrocarSenhaCommand(Guid AccountId, string SenhaAtual, string NovaSenha);
