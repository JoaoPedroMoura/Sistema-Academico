using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class PreRequisitoConfiguration : IEntityTypeConfiguration<PreRequisito>
{
    public void Configure(EntityTypeBuilder<PreRequisito> builder)
    {
        builder.ToTable("PreRequisitos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.HasIndex(p => new { p.MateriaId, p.MateriaRequisitoId }).IsUnique();

        builder.HasOne<Materia>().WithMany().HasForeignKey(p => p.MateriaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Materia>().WithMany().HasForeignKey(p => p.MateriaRequisitoId).OnDelete(DeleteBehavior.Restrict);
    }
}
