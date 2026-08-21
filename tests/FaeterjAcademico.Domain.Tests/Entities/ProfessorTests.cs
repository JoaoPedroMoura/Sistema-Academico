using FaeterjAcademico.Domain.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Domain.Tests.Entities;

public class ProfessorTests
{
    private static Professor CriarProfessor() =>
        new(Guid.NewGuid(), "João Pedro", "joao@faeterj.edu.br");

    [Fact]
    public void AdicionarDisponibilidade_SemColisao_Adiciona()
    {
        var professor = CriarProfessor();

        professor.AdicionarDisponibilidade(new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(12, 30)));

        Assert.Single(professor.Disponibilidades);
    }

    [Fact]
    public void AdicionarDisponibilidade_ComColisao_LancaExcecao()
    {
        // Base da restrição rígida 4: disponibilidade não pode se sobrepor a si mesma
        // (o motor GRASP consulta este conjunto para validar onde o professor pode ser alocado).
        var professor = CriarProfessor();
        professor.AdicionarDisponibilidade(new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(9, 0)));

        Assert.Throws<DomainException>(() =>
            professor.AdicionarDisponibilidade(new HorarioSlot(DiaSemana.Segunda, new TimeOnly(8, 0), new TimeOnly(10, 0))));
    }

    [Fact]
    public void Construtor_SemNome_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => new Professor(Guid.NewGuid(), "", "joao@faeterj.edu.br"));
    }
}
