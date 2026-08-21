using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;

public sealed record AdicionarDisponibilidadeCommand(Guid ProfessorId, DiaSemana Dia, TimeOnly HoraInicio, TimeOnly HoraFim);
