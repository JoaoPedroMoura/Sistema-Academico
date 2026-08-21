using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Schedule.Dtos;
using FaeterjAcademico.Domain.Entities;
using FaeterjAcademico.Domain.Scheduling;

namespace FaeterjAcademico.Application.Schedule.GerarGrade;

/// <summary>
/// Caso de uso "Manter Grade de Horário / Gerar" do TCC original (ANALISE-TCC.md §4, UC7),
/// ligando o motor GRASP (Fase 5, puro domínio) aos dados reais do tenant. Publica a grade se o
/// GRASP alocou todas as matérias; senão mantém como rascunho e devolve quais matérias ficaram
/// de fora, para o Admin ajustar disponibilidade/vínculos e gerar de novo.
/// </summary>
public sealed class GerarGradeHandler(
    IAcademicoRepository repository,
    IScheduleGenerator scheduleGenerator,
    ICurrentUserAccessor currentUser) : IRequestHandler<GerarGradeCommand, GerarGradeResultDto>
{
    public async Task<GerarGradeResultDto> HandleAsync(GerarGradeCommand request, CancellationToken cancellationToken = default)
    {
        var professores = await repository.GetProfessoresAsync(cancellationToken);
        var materias = await repository.GetMateriasAsync(cancellationToken);
        var vinculos = await repository.GetVinculosAsync(cancellationToken);
        var periodosAula = await repository.GetPeriodosAulaAsync(cancellationToken);

        if (periodosAula.Count == 0)
        {
            throw new UseCaseException("Não há períodos de aula cadastrados para esta unidade.");
        }

        var materiasAtivas = materias.Where(m => m.Ativa).ToList();
        if (materiasAtivas.Count == 0)
        {
            throw new UseCaseException("Não há matérias ativas para gerar a grade.");
        }

        var input = new ScheduleGenerationInput(
            Materias: [.. materiasAtivas.Select(m => new MateriaInput(m.Id, m.Periodo, m.CargaHorariaSemanal))],
            Professores: [.. professores.Where(p => p.Ativo).Select(p =>
                new ProfessorInput(p.Id, [.. p.Disponibilidades.Select(d => d.Slot)]))],
            Vinculos: [.. vinculos.Select(v => new VinculoInput(v.MateriaId, v.ProfessorId))],
            PeriodosAula: [.. periodosAula.Select(pa => new PeriodoAulaInput(pa.Id, pa.Slot))]);

        var options = request.Iterations is > 0 ? new GraspOptions { Iterations = request.Iterations.Value } : null;
        var resultado = scheduleGenerator.Generate(input, options);

        var grade = new Grade();
        foreach (var turma in resultado.Turmas)
        {
            grade.AdicionarTurma(turma.MateriaId, turma.ProfessorId, turma.PeriodoAulaId, turma.Slot, turma.PeriodoCurricular);
        }

        if (resultado.Completa)
        {
            grade.Publicar(resultado.Custo);
        }

        await repository.AddGradeAsync(grade, cancellationToken);
        await repository.AddLogAsync(
            new LogSistema(
                currentUser.AccountId,
                "Grade.Gerar",
                "Grade",
                grade.Id,
                sucesso: resultado.Completa,
                resultado.Completa ? null : $"{resultado.MateriasNaoAlocadas.Count} matéria(s) não alocada(s)."),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var materiasPorId = materias.ToDictionary(m => m.Id);
        var professoresPorId = professores.ToDictionary(p => p.Id);
        var gradeDto = ToDto(grade, materiasPorId, professoresPorId);
        var nomesNaoAlocadas = resultado.MateriasNaoAlocadas
            .Select(id => materiasPorId.TryGetValue(id, out var m) ? m.Nome : id.ToString())
            .ToList();

        return new GerarGradeResultDto(gradeDto, resultado.Completa, nomesNaoAlocadas);
    }

    internal static GradeDto ToDto(
        Grade grade,
        IReadOnlyDictionary<Guid, Materia> materiasPorId,
        IReadOnlyDictionary<Guid, Professor> professoresPorId) => new(
        grade.Id,
        grade.Status.ToString(),
        grade.GeradoEmUtc,
        grade.CustoSolucao,
        [.. grade.Turmas
            .Where(t => materiasPorId.ContainsKey(t.MateriaId) && professoresPorId.ContainsKey(t.ProfessorId))
            .Select(t => new TurmaDto(
                t.Id,
                t.MateriaId,
                materiasPorId[t.MateriaId].Nome,
                t.ProfessorId,
                professoresPorId[t.ProfessorId].Nome,
                t.Slot.Dia.ToString(),
                t.Slot.HoraInicio.ToString("HH:mm"),
                t.Slot.HoraFim.ToString("HH:mm"),
                t.PeriodoCurricular))]);
}
