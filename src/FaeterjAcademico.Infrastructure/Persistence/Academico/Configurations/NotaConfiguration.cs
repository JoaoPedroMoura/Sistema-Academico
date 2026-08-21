using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class NotaConfiguration : IEntityTypeConfiguration<Nota>
{
    public void Configure(EntityTypeBuilder<Nota> builder)
    {
        builder.ToTable("Notas");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder.Property(n => n.Tipo).HasMaxLength(100).IsRequired();
        builder.Property(n => n.Valor).HasPrecision(4, 2).IsRequired();

        builder.HasOne<Aluno>().WithMany().HasForeignKey(n => n.AlunoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Turma>().WithMany().HasForeignKey(n => n.TurmaId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(n => new { n.AlunoId, n.TurmaId });
    }
}
