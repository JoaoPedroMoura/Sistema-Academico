namespace FaeterjAcademico.Api.Controllers.Students;

public sealed record CriarAlunoRequest(string Nome, string Email, string Matricula, int PeriodoAtual);

public sealed record AtualizarAlunoRequest(int NovoPeriodo);
