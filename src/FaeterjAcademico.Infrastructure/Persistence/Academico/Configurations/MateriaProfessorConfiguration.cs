using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class MateriaProfessorConfiguration : IEntityTypeConfiguration<MateriaProfessor>
{
    public void Configure(EntityTypeBuilder<MateriaProfessor> builder)
    {
        builder.ToTable("MateriaProfessores");
        builder.HasKey(mp => mp.Id);
        builder.Property(mp => mp.Id).ValueGeneratedNever();

        builder.HasIndex(mp => new { mp.MateriaId, mp.ProfessorId }).IsUnique();

        builder.HasOne<Materia>().WithMany().HasForeignKey(mp => mp.MateriaId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Professor>().WithMany().HasForeignKey(mp => mp.ProfessorId).OnDelete(DeleteBehavior.Cascade);
    }
}
