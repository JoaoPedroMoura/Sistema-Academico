using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class LogSistemaConfiguration : IEntityTypeConfiguration<LogSistema>
{
    public void Configure(EntityTypeBuilder<LogSistema> builder)
    {
        builder.ToTable("LogsSistema");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.Acao).HasMaxLength(200).IsRequired();
        builder.Property(l => l.EntidadeTipo).HasMaxLength(100).IsRequired();
        builder.Property(l => l.Detalhes).HasMaxLength(2000);

        builder.HasIndex(l => l.DataHoraUtc);
        builder.HasIndex(l => new { l.EntidadeTipo, l.EntidadeId });
    }
}
