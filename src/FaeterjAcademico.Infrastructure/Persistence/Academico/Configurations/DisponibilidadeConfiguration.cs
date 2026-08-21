using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class DisponibilidadeConfiguration : IEntityTypeConfiguration<Disponibilidade>
{
    public void Configure(EntityTypeBuilder<Disponibilidade> builder)
    {
        builder.ToTable("Disponibilidades");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).ValueGeneratedNever();

        builder.ComplexProperty(d => d.Slot, slot =>
        {
            slot.Property(s => s.Dia).HasColumnName("Dia").IsRequired();
            slot.Property(s => s.HoraInicio).HasColumnName("HoraInicio").IsRequired();
            slot.Property(s => s.HoraFim).HasColumnName("HoraFim").IsRequired();
        });
    }
}
