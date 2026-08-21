using FaeterjAcademico.Domain.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Domain.Tests.Entities;

/// <summary>
/// Cobre as 3 restrições rígidas que pertencem ao agregado Grade (ANALISE-TCC.md §1).
/// A restrição 4 (disponibilidade do professor) é coberta em ProfessorTests, pois pertence ao
/// agregado Professor.
/// </summary>
public class GradeTests
{
    private static readonly HorarioSlot SegundaPrimeiroTempo = new(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(7, 50));
    private static readonly HorarioSlot SegundaPrimeiroTempoDuplicado = new(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(7, 50));
    private static readonly HorarioSlot SegundaSegundoTempo = new(DiaSemana.Segunda, new TimeOnly(7, 50), new TimeOnly(8, 40));

    [Fact]
    public void AdicionarTurma_ComDadosValidos_AdicionaNaGrade()
    {
        var grade = new Grade();

        var turma = grade.AdicionarTurma(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SegundaPrimeiroTempo, periodoCurricular: 1);

        Assert.Single(grade.Turmas);
        Assert.Equal(grade.Id, turma.GradeId);
    }

    [Fact]
    public void AdicionarTurma_ProfessorComHorarioColidente_LancaExcecao()
    {
        // Restrição rígida 1: professor não pode lecionar 2 disciplinas no mesmo horário.
        var grade = new Grade();
        var professorId = Guid.NewGuid();
        grade.AdicionarTurma(Guid.NewGuid(), professorId, Guid.NewGuid(), SegundaPrimeiroTempo, periodoCurricular: 1);

        var ex = Assert.Throws<DomainException>(() =>
            grade.AdicionarTurma(Guid.NewGuid(), professorId, Guid.NewGuid(), SegundaPrimeiroTempoDuplicado, periodoCurricular: 2));

        Assert.Contains("restrição rígida 1", ex.Message);
    }

    [Fact]
    public void AdicionarTurma_PeriodoCurricularComHorarioColidente_LancaExcecao()
    {
        // Restrição rígida 2: uma turma (período curricular) não pode ter 2 aulas no mesmo horário.
        var grade = new Grade();
        grade.AdicionarTurma(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SegundaPrimeiroTempo, periodoCurricular: 1);

        var ex = Assert.Throws<DomainException>(() =>
            grade.AdicionarTurma(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), SegundaPrimeiroTempoDuplicado, periodoCurricular: 1));

        Assert.Contains("restrição rígida 2", ex.Message);
    }

    [Fact]
    public void AdicionarTurma_MateriaComOutroProfessorNaGrade_LancaExcecao()
    {
        // Restrição rígida 3: disciplina não pode ter mais de um professor alocado simultaneamente.
        var grade = new Grade();
        var materiaId = Guid.NewGuid();
        grade.AdicionarTurma(materiaId, Guid.NewGuid(), Guid.NewGuid(), SegundaPrimeiroTempo, periodoCurricular: 1);

        var ex = Assert.Throws<DomainException>(() =>
            grade.AdicionarTurma(materiaId, Guid.NewGuid(), Guid.NewGuid(), SegundaSegundoTempo, periodoCurricular: 2));

        Assert.Contains("restrição rígida 3", ex.Message);
    }

    [Fact]
    public void AdicionarTurma_MesmoProfessorMesmaMateriaHorarioDiferente_NaoLancaExcecao()
    {
        // Mesmo professor pode ter mais de um tempo da mesma matéria (aulas consecutivas).
        var grade = new Grade();
        var materiaId = Guid.NewGuid();
        var professorId = Guid.NewGuid();
        grade.AdicionarTurma(materiaId, professorId, Guid.NewGuid(), SegundaPrimeiroTempo, periodoCurricular: 1);

        grade.AdicionarTurma(materiaId, professorId, Guid.NewGuid(), SegundaSegundoTempo, periodoCurricular: 1);

        Assert.Equal(2, grade.Turmas.Count);
    }

    [Fact]
    public void Publicar_DefineStatusPublicadaECustoSolucao()
    {
        var grade = new Grade();

        grade.Publicar(custoSolucao: 3.5);

        Assert.Equal(GradeStatus.Publicada, grade.Status);
        Assert.Equal(3.5, grade.CustoSolucao);
    }
}
