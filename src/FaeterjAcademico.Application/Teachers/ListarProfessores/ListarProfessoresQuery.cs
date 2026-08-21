namespace FaeterjAcademico.Application.Teachers.ListarProfessores;

/// <summary>Pesquisa opcional por nome/email (caso de uso "Pesquisar" do TCC original).</summary>
public sealed record ListarProfessoresQuery(string? Pesquisa = null);
