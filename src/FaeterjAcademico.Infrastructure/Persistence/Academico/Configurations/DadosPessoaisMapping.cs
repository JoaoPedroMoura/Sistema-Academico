using FaeterjAcademico.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

/// <summary>Colunas de <see cref="DadosPessoais"/>, iguais em Alunos e Professores (sem prefixo).</summary>
internal static class DadosPessoaisMapping
{
    public static void Configure(ComplexPropertyBuilder<DadosPessoais> d)
    {
        d.Property(x => x.Cpf).HasColumnName("Cpf").HasMaxLength(11);
        d.Property(x => x.RgNumero).HasColumnName("RgNumero").HasMaxLength(20);
        d.Property(x => x.RgOrgaoEmissor).HasColumnName("RgOrgaoEmissor").HasMaxLength(20);
        d.Property(x => x.RgUf).HasColumnName("RgUf").HasMaxLength(2);
        d.Property(x => x.RgDataEmissao).HasColumnName("RgDataEmissao");
        d.Property(x => x.DataNascimento).HasColumnName("DataNascimento");
        d.Property(x => x.Naturalidade).HasColumnName("Naturalidade").HasMaxLength(100);
        d.Property(x => x.NomeMae).HasColumnName("NomeMae").HasMaxLength(200);
        d.Property(x => x.NomePai).HasColumnName("NomePai").HasMaxLength(200);
        d.Property(x => x.Cep).HasColumnName("Cep").HasMaxLength(8);
        d.Property(x => x.Logradouro).HasColumnName("Logradouro").HasMaxLength(200);
        d.Property(x => x.Numero).HasColumnName("Numero").HasMaxLength(20);
        d.Property(x => x.Complemento).HasColumnName("Complemento").HasMaxLength(100);
        d.Property(x => x.Bairro).HasColumnName("Bairro").HasMaxLength(100);
        d.Property(x => x.Cidade).HasColumnName("Cidade").HasMaxLength(100);
        d.Property(x => x.Uf).HasColumnName("Uf").HasMaxLength(2);
    }
}
