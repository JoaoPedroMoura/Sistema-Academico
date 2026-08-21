using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class TurmaConfiguration : IEntityTypeConfiguration<Turma>
{
    public void Configure(EntityTypeBuilder<Turma> builder)
    {
        builder.ToTable("Turmas");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedNever();

        builder.Property(t => t.PeriodoCurricular).IsRequired();

        builder.HasOne<Materia>().WithMany().HasForeignKey(t => t.MateriaId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Professor>().WithMany().HasForeignKey(t => t.ProfessorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<PeriodoAula>().WithMany().HasForeignKey(t => t.PeriodoAulaId).OnDelete(DeleteBehavior.Restrict);

        builder.ComplexProperty(t => t.Slot, slot =>
        {
            slot.Property(s => s.Dia).HasColumnName("Dia").IsRequired();
            slot.Property(s => s.HoraInicio).HasColumnName("HoraInicio").IsRequired();
            slot.Property(s => s.HoraFim).HasColumnName("HoraFim").IsRequired();
        });
    }
}
