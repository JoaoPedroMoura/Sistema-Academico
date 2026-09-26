using FaeterjAcademico.Domain.Common;

namespace FaeterjAcademico.Domain.Tests.Common;

public class DadosPessoaisTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("52998224725", true)]
    [InlineData("529.982.247-24", false)]
    [InlineData("111.111.111-11", false)]
    [InlineData("1234", false)]
    public void CpfValido_ChecaDigitosVerificadores(string cpf, bool esperado)
    {
        var digitos = new string(cpf.Where(char.IsAsciiDigit).ToArray());
        Assert.Equal(esperado, DadosPessoais.CpfValido(digitos));
    }

    [Fact]
    public void Normalizar_LimpaMascarasEVazios()
    {
        var dados = new DadosPessoais(Cpf: "529.982.247-25", Cep: "25685-000", Uf: "rj", NomeMae: "  ").Normalizar();

        Assert.Equal("52998224725", dados.Cpf);
        Assert.Equal("25685000", dados.Cep);
        Assert.Equal("RJ", dados.Uf);
        Assert.Null(dados.NomeMae);
    }

    [Fact]
    public void Normalizar_CpfInvalido_LancaExcecao()
    {
        Assert.Throws<DomainException>(() => new DadosPessoais(Cpf: "123.456.789-00").Normalizar());
    }
}
