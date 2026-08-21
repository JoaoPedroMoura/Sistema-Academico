using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class AlunoConfiguration : IEntityTypeConfiguration<Aluno>
{
    public void Configure(EntityTypeBuilder<Aluno> builder)
    {
        builder.ToTable("Alunos");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Nome).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Email).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Matricula).HasMaxLength(50).IsRequired();

        builder.HasIndex(a => a.Email).IsUnique();
        builder.HasIndex(a => a.Matricula).IsUnique();
        builder.HasIndex(a => a.AccountId).IsUnique();
    }
}
