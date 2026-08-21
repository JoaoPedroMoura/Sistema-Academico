namespace FaeterjAcademico.Application.Subjects.Dtos;

public sealed record MateriaDto(Guid Id, string Nome, int Periodo, int CargaHorariaSemanal, bool Ativa)
{
    public static MateriaDto FromEntity(Domain.Entities.Materia materia) =>
        new(materia.Id, materia.Nome, materia.Periodo, materia.CargaHorariaSemanal, materia.Ativa);
}
