using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Api.Controllers.Students;

public sealed record CriarAlunoRequest(
    string Nome, string Email, int PeriodoAtual, DadosPessoais? DadosPessoais = null, DocumentosAluno? Documentos = null);

public sealed record AtualizarAlunoRequest(
    string Nome, int PeriodoAtual, DadosPessoais? DadosPessoais = null, DocumentosAluno? Documentos = null);
