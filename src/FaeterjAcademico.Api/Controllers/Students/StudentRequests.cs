namespace FaeterjAcademico.Api.Controllers.Students;

public sealed record CriarAlunoRequest(string Nome, string Email, int PeriodoAtual);

public sealed record AtualizarAlunoRequest(string Nome, int PeriodoAtual);
