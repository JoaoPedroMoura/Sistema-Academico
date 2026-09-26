using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Teachers.AtualizarProfessor;

public sealed record AtualizarProfessorCommand(
    Guid Id, string Nome, string? Telefone, DadosPessoais? DadosPessoais = null, FormacaoProfessor? Formacao = null);
