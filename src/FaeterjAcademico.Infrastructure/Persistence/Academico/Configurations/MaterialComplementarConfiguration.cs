using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class MaterialComplementarConfiguration : IEntityTypeConfiguration<MaterialComplementar>
{
    public void Configure(EntityTypeBuilder<MaterialComplementar> builder)
    {
        builder.ToTable("MateriaisComplementares");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Titulo).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Descricao).HasMaxLength(1000);
        builder.Property(m => m.ArquivoUrl).IsRequired();
        builder.Property(m => m.ArquivoNomeOriginal).HasMaxLength(260).IsRequired();

        builder.HasOne<Turma>().WithMany().HasForeignKey(m => m.TurmaId).OnDelete(DeleteBehavior.Cascade);
    }
}
