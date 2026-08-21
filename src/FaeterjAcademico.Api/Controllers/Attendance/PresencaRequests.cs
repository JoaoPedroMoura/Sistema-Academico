using FaeterjAcademico.Application.Attendance.Dtos;

namespace FaeterjAcademico.Api.Controllers.Attendance;

public sealed record RegistrarPresencaRequest(Guid TurmaId, DateOnly DataAula, IReadOnlyList<RegistroPresencaInput> Registros);
