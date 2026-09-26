using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class DocumentoAnexoConfiguration : IEntityTypeConfiguration<DocumentoAnexo>
{
    public void Configure(EntityTypeBuilder<DocumentoAnexo> builder)
    {
        builder.ToTable("DocumentosAnexos");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        // Sem FK: PessoaId aponta para Alunos ou Professores (validado na Application).
        builder.Property(d => d.Tipo).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(d => d.NomeArquivo).HasMaxLength(255).IsRequired();
        builder.Property(d => d.ContentType).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Conteudo).IsRequired();

        builder.HasIndex(d => d.PessoaId);
    }
}
