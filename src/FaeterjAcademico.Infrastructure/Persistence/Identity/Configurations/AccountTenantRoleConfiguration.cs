using FaeterjAcademico.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaeterjAcademico.Infrastructure.Persistence.Identity.Configurations;

public class AccountTenantRoleConfiguration : IEntityTypeConfiguration<AccountTenantRole>
{
    public void Configure(EntityTypeBuilder<AccountTenantRole> builder)
    {
        builder.ToTable("AccountTenantRoles");
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.HasIndex(v => new { v.AccountId, v.TenantId, v.Role }).IsUnique();

        builder
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(v => v.TenantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
