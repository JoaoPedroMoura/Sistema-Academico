using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Scheduling;
using FaeterjAcademico.Infrastructure.Auth;
using FaeterjAcademico.Infrastructure.MultiTenancy;
using FaeterjAcademico.Infrastructure.Persistence.Academico;
using FaeterjAcademico.Infrastructure.Persistence.Identity;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Finbuckle.MultiTenant.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FaeterjAcademico.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default não configurada.");

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityRepository, IdentityRepository>();
        services.AddScoped<IAcademicoRepository, AcademicoRepository>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserAccessor, CurrentUserAccessor>();
        services.AddScoped<ICurrentTenantAccessor, CurrentTenantAccessor>();

        // Motor GRASP (Fase 5) — stateless, sem dependências, pode ser singleton.
        services.AddSingleton<IScheduleGenerator, GraspScheduleGenerator>();

        // Schema fixo "identity" (control-plane, único — ARCHITECTURE.md §3.2). Sem multi-tenancy.
        services.AddDbContext<IdentityDbContext>(options => options.UseNpgsql(connectionString));

        // Resolução de tenant: header em dev (X-Tenant-Slug), subdomínio em produção — ambas as
        // estratégias registradas, avaliadas nessa ordem (ARCHITECTURE.md §3.3).
        services
            .AddMultiTenant<AppTenantInfo>()
            .WithHeaderStrategy("X-Tenant-Slug")
            .WithHostStrategy()
            .WithStore<IdentityDbTenantStore>(ServiceLifetime.Scoped);

        // Dados acadêmicos, isolados por tenant via search_path da conexão (ARCHITECTURE.md
        // §3.4) — o schema é resolvido a partir do tenant já identificado pelo Finbuckle para a
        // requisição atual (middleware UseMultiTenant roda antes de qualquer DbContext ser
        // resolvido pela injeção de dependência do controller).
        services.AddDbContext<AcademicoDbContext>((sp, options) =>
        {
            var tenantAccessor = sp.GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>();
            var schemaName = tenantAccessor.MultiTenantContext?.TenantInfo?.SchemaName
                ?? throw new InvalidOperationException(
                    "Nenhum tenant resolvido para a requisição atual — verifique o header " +
                    "X-Tenant-Slug (dev) ou o subdomínio (produção).");

            AcademicoDbContextOptions.Configure(options, connectionString, schemaName);
        });

        return services;
    }
}
