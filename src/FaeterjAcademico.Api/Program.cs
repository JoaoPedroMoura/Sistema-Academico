using System.Text;
using FaeterjAcademico.Application;
using FaeterjAcademico.Infrastructure;
using FaeterjAcademico.Infrastructure.Auth;
using FaeterjAcademico.Infrastructure.MultiTenancy;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

const string FrontendCorsPolicy = "Frontend";
var frontendOrigin = builder.Configuration["Frontend:Origin"] ?? "http://localhost:3000";

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options => options.AddPolicy(FrontendCorsPolicy, policy => policy
    .WithOrigins(frontendOrigin)
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials())); // necessário para o cookie httpOnly de refresh token

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });

// Autenticado por padrão em todo endpoint — quem precisa ser público usa [AllowAnonymous]
// explicitamente (ex. AuthController). Seguro por padrão em vez de opt-in.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Tradução central de exceções para status HTTP — evita try/catch repetido em cada controller.
// DomainException (regra de negócio, ex. restrição rígida do GRASP) => 422;
// AuthenticationFailedException => 401; UseCaseException genérica => 400.
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (FaeterjAcademico.Domain.Common.DomainException ex)
    {
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
    catch (FaeterjAcademico.Application.Common.AuthenticationFailedException ex)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
    catch (FaeterjAcademico.Application.Common.UseCaseException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { message = ex.Message });
    }
});

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);

// Resolve o tenant (header X-Tenant-Slug em dev, subdomínio em produção — ARCHITECTURE.md §3.3)
// antes de autenticação/autorização, para que o AcademicoDbContext já tenha o schema certo
// quando um controller for resolvido.
app.UseMultiTenant();

app.UseAuthentication();

// Trava de segurança (ARCHITECTURE.md §3.3): o claim "tenant" do JWT precisa bater com o tenant
// resolvido para a requisição (header/subdomínio). Sem isso, um token válido de uma unidade
// poderia ler/escrever dados de outra só trocando o header X-Tenant-Slug — [Authorize(Roles=...)]
// sozinho não impede isso, porque papel e tenant são checados por mecanismos diferentes.
// Login/refresh/logout não passam por aqui: não têm tenant resolvido ainda (é o que eles
// estabelecem) nem o [AllowAnonymous] do AuthController.
app.Use(async (context, next) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        var tenantDoToken = context.User.FindFirst("tenant")?.Value;
        var tenantResolvido = context.GetTenantInfo<AppTenantInfo>()?.Identifier;

        // Bloqueia tanto "resolveu para outro tenant" quanto "não resolveu nenhum" (header
        // ausente ou apontando para um slug inexistente) — as duas são o mesmo risco: um
        // endpoint autenticado sendo acessado sem o tenant do próprio token confirmado.
        if (tenantResolvido is null ||
            !string.Equals(tenantDoToken, tenantResolvido, StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { message = "Token não pertence a esta unidade." });
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapControllers();

app.Run();
