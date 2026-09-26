using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Teachers.CriarProfessor;

public sealed record CriarProfessorCommand(
    string Nome, string Email, string? Telefone, DadosPessoais? DadosPessoais = null, FormacaoProfessor? Formacao = null);
