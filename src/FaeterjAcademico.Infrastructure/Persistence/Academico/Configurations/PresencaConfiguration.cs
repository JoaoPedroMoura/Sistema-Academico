using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class PresencaConfiguration : IEntityTypeConfiguration<Presenca>
{
    public void Configure(EntityTypeBuilder<Presenca> builder)
    {
        builder.ToTable("Presencas");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Justificativa).HasMaxLength(500);

        builder.HasOne<Aluno>().WithMany().HasForeignKey(p => p.AlunoId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Turma>().WithMany().HasForeignKey(p => p.TurmaId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.AlunoId, p.TurmaId, p.DataAula }).IsUnique();
    }
}
