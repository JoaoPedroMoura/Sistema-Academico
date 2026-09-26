namespace FaeterjAcademico.Domain.Common;

/// <summary>
/// Documentos e dados pessoais comuns a aluno e professor. Tudo opcional (dá pra completar depois
/// do cadastro); o que vier preenchido é normalizado e validado por <see cref="Normalizar"/>.
/// </summary>
public sealed record DadosPessoais(
    string? Cpf = null,
    string? RgNumero = null,
    string? RgOrgaoEmissor = null,
    string? RgUf = null,
    DateOnly? RgDataEmissao = null,
    DateOnly? DataNascimento = null,
    string? Naturalidade = null,
    string? NomeMae = null,
    string? NomePai = null,
    string? Cep = null,
    string? Logradouro = null,
    string? Numero = null,
    string? Complemento = null,
    string? Bairro = null,
    string? Cidade = null,
    string? Uf = null)
{
    public static readonly DadosPessoais Vazio = new();

    /// <summary>Trim, vazio vira null, CPF/CEP só dígitos, UF maiúscula — e valida o formato.</summary>
    public DadosPessoais Normalizar()
    {
        var cpf = SoDigitos(Cpf);
        if (cpf is not null && !CpfValido(cpf))
        {
            throw new DomainException("CPF inválido.");
        }
        var cep = SoDigitos(Cep);
        if (cep is not null && cep.Length != 8)
        {
            throw new DomainException("CEP deve ter 8 dígitos.");
        }
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        if (DataNascimento > hoje || RgDataEmissao > hoje)
        {
            throw new DomainException("Datas não podem estar no futuro.");
        }

        return new DadosPessoais(
            cpf, Limpar(RgNumero), Limpar(RgOrgaoEmissor), ValidarUf(RgUf), RgDataEmissao,
            DataNascimento, Limpar(Naturalidade), Limpar(NomeMae), Limpar(NomePai),
            cep, Limpar(Logradouro), Limpar(Numero), Limpar(Complemento), Limpar(Bairro), Limpar(Cidade), ValidarUf(Uf));
    }

    public static bool CpfValido(string cpf)
    {
        if (cpf.Length != 11 || !cpf.All(char.IsAsciiDigit) || cpf.Distinct().Count() == 1)
        {
            return false;
        }

        int Digito(int tamanho)
        {
            var soma = 0;
            for (var i = 0; i < tamanho; i++)
            {
                soma += (cpf[i] - '0') * (tamanho + 1 - i);
            }
            var resto = soma * 10 % 11;
            return resto == 10 ? 0 : resto;
        }

        return Digito(9) == cpf[9] - '0' && Digito(10) == cpf[10] - '0';
    }

    internal static string? Limpar(string? valor) => string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

    private static string? SoDigitos(string? valor) =>
        Limpar(valor) is { } v ? new string(v.Where(char.IsAsciiDigit).ToArray()) : null;

    private static string? ValidarUf(string? uf)
    {
        var valor = Limpar(uf)?.ToUpperInvariant();
        if (valor is not null && (valor.Length != 2 || !valor.All(char.IsAsciiLetter)))
        {
            throw new DomainException("UF deve ter 2 letras.");
        }
        return valor;
    }
}
