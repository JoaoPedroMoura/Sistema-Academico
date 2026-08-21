namespace FaeterjAcademico.Api.Controllers.Subjects;

public sealed record CriarMateriaRequest(string Nome, int Periodo, int CargaHorariaSemanal);

public sealed record AtualizarMateriaRequest(string Nome, int Periodo, int CargaHorariaSemanal);

public sealed record AdicionarVinculoRequest(Guid ProfessorId);
