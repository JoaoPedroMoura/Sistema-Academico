using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Schedule.Dtos;

namespace FaeterjAcademico.Application.Schedule.ObterGradeAtiva;

/// <summary>Consumida também pela Secretaria e por qualquer papel que só precisa visualizar a grade publicada.</summary>
public sealed class ObterGradeAtivaHandler(IAcademicoRepository repository)
    : IRequestHandler<ObterGradeAtivaQuery, GradeDto?>
{
    public async Task<GradeDto?> HandleAsync(ObterGradeAtivaQuery request, CancellationToken cancellationToken = default)
    {
        var grade = await repository.GetGradeAtivaAsync(cancellationToken);
        if (grade is null)
        {
            return null;
        }

        var materias = (await repository.GetMateriasAsync(cancellationToken)).ToDictionary(m => m.Id);
        var professores = (await repository.GetProfessoresAsync(cancellationToken)).ToDictionary(p => p.Id);

        return GerarGrade.GerarGradeHandler.ToDto(grade, materias, professores);
    }
}
