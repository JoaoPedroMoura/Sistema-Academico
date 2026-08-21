using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class PeriodoAulaConfiguration : IEntityTypeConfiguration<PeriodoAula>
{
    public void Configure(EntityTypeBuilder<PeriodoAula> builder)
    {
        builder.ToTable("PeriodosAula");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(p => p.Ordem).IsRequired();

        builder.ComplexProperty(p => p.Slot, slot =>
        {
            slot.Property(s => s.Dia).HasColumnName("Dia").IsRequired();
            slot.Property(s => s.HoraInicio).HasColumnName("HoraInicio").IsRequired();
            slot.Property(s => s.HoraFim).HasColumnName("HoraFim").IsRequired();
        });
    }
}
