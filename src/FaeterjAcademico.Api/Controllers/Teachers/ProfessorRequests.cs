using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Api.Controllers.Teachers;

public sealed record CriarProfessorRequest(string Nome, string Email, string? Telefone);

public sealed record AtualizarProfessorRequest(string Nome, string Email, string? Telefone);

public sealed record AdicionarDisponibilidadeRequest(DiaSemana Dia, TimeOnly HoraInicio, TimeOnly HoraFim);
