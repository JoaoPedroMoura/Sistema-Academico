using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Tests.Common;

public class HorarioSlotTests
{
    [Fact]
    public void Colide_MesmoDiaComSobreposicao_RetornaTrue()
    {
        var a = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(8, 0));
        var b = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 30), new TimeOnly(8, 30));

        Assert.True(a.Colide(b));
    }

    [Fact]
    public void Colide_MesmoDiaSemSobreposicao_RetornaFalse()
    {
        var a = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(8, 0));
        var b = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(8, 0), new TimeOnly(9, 0));

        Assert.False(a.Colide(b));
    }

    [Fact]
    public void Colide_DiasDiferentes_RetornaFalse()
    {
        var a = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(8, 0));
        var b = new HorarioSlot(DiaSemana.Terca, new TimeOnly(7, 0), new TimeOnly(8, 0));

        Assert.False(a.Colide(b));
    }

    [Fact]
    public void EhConsecutivoA_FimBateComInicio_RetornaTrue()
    {
        var a = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(7, 0), new TimeOnly(8, 0));
        var b = new HorarioSlot(DiaSemana.Segunda, new TimeOnly(8, 0), new TimeOnly(9, 0));

        Assert.True(a.EhConsecutivoA(b));
    }

    [Fact]
    public void Construtor_HoraFimAntesDeHoraInicio_LancaExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new HorarioSlot(DiaSemana.Segunda, new TimeOnly(9, 0), new TimeOnly(8, 0)));
    }
}
