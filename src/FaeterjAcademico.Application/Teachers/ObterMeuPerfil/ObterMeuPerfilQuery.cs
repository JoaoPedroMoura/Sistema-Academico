namespace FaeterjAcademico.Application.Teachers.ObterMeuPerfil;

/// <summary><paramref name="AccountId"/> vem do claim do JWT — o professor nunca informa "quem ele é".</summary>
public sealed record ObterMeuPerfilQuery(Guid AccountId);
