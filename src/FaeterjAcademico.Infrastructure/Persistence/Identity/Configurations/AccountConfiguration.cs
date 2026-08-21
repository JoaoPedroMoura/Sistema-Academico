using FaeterjAcademico.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity.Configurations;

public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();

        builder.Property(a => a.Nome).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Email).HasMaxLength(200).IsRequired();
        builder.HasIndex(a => a.Email).IsUnique();
        builder.Property(a => a.SenhaHash).IsRequired();

        builder
            .HasMany(a => a.Vinculos)
            .WithOne()
            .HasForeignKey(v => v.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Account.Vinculos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
