using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico.Configurations;

public class SolicitacaoConfiguration : IEntityTypeConfiguration<Solicitacao>
{
    public void Configure(EntityTypeBuilder<Solicitacao> builder)
    {
        builder.ToTable("Solicitacoes");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Tipo).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(s => s.Descricao).HasMaxLength(2000).IsRequired();
        builder.Property(s => s.Resposta).HasMaxLength(2000);

        builder.HasOne<Aluno>().WithMany().HasForeignKey(s => s.AlunoId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.Status);
    }
}
