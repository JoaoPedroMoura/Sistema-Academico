using FaeterjAcademico.Domain.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Domain.Tests.Entities;

public class AlunoTests
{
    private static Aluno CriarAluno() =>
        new(Guid.NewGuid(), "Maria", "maria@faeterj.edu.br", "2026112345", 1);

    [Fact]
    public void AtualizarDados_Validos_AtualizaEPreservaIdentificadores()
    {
        var aluno = CriarAluno();

        aluno.AtualizarDados("  Maria Silva ", 3);

        Assert.Equal("Maria Silva", aluno.Nome);
        Assert.Equal(3, aluno.PeriodoAtual);
        Assert.Equal("maria@faeterj.edu.br", aluno.Email);
        Assert.Equal("2026112345", aluno.Matricula);
    }

    [Theory]
    [InlineData("", 1)]
    [InlineData("Maria", 0)]
    public void AtualizarDados_Invalidos_LancaExcecao(string nome, int periodo)
    {
        Assert.Throws<DomainException>(() => CriarAluno().AtualizarDados(nome, periodo));
    }

    [Theory]
    [InlineData(2026, 1, 1, "20261")]
    [InlineData(2026, 6, 30, "20261")]
    [InlineData(2026, 7, 1, "20262")]
    [InlineData(2026, 12, 31, "20262")]
    public void PrefixoMatricula_UsaAnoESemestre(int ano, int mes, int dia, string esperado)
    {
        Assert.Equal(esperado, Aluno.PrefixoMatricula(new DateOnly(ano, mes, dia)));
    }
}
