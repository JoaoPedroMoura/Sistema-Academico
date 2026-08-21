namespace FaeterjAcademico.Application.Subjects.AtualizarMateria;

public sealed record AtualizarMateriaCommand(Guid Id, string Nome, int Periodo, int CargaHorariaSemanal);
