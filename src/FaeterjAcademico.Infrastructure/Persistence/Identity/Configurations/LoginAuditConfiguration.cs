using FaeterjAcademico.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity.Configurations;

public class LoginAuditConfiguration : IEntityTypeConfiguration<LoginAudit>
{
    public void Configure(EntityTypeBuilder<LoginAudit> builder)
    {
        builder.ToTable("LoginAudits");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever();

        builder.Property(l => l.EmailTentativa).HasMaxLength(200).IsRequired();
        builder.Property(l => l.IpAddress).HasMaxLength(64);
        builder.HasIndex(l => l.DataHoraUtc);
    }
}
