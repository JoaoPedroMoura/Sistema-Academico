using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class ProfessorConfiguration : IEntityTypeConfiguration<Professor>
{
    public void Configure(EntityTypeBuilder<Professor> builder)
    {
        builder.ToTable("Professores");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Nome).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(p => p.Email).IsUnique();
        builder.HasIndex(p => p.AccountId).IsUnique();

        builder
            .HasMany(p => p.Disponibilidades)
            .WithOne()
            .HasForeignKey(d => d.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Professor.Disponibilidades))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
