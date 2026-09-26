using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Students.AtualizarAluno;

public sealed record AtualizarAlunoCommand(
    Guid Id, string Nome, int PeriodoAtual, DadosPessoais? DadosPessoais = null, DocumentosAluno? Documentos = null);
