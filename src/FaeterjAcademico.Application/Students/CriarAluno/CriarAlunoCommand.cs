using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Students.CriarAluno;

public sealed record CriarAlunoCommand(
    string Nome, string Email, int PeriodoAtual, DadosPessoais? DadosPessoais = null, DocumentosAluno? Documentos = null);
