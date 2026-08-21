# Persistence

Dois `DbContext` distintos (ver [ARCHITECTURE.md §3](../../../ARCHITECTURE.md)):

- `Identity/IdentityDbContext.cs` — schema `identity` (control-plane): `Tenant`, `Account`,
  `AccountTenantRole`, `RefreshToken`, `LoginAudit`. Único, sem multi-tenancy.
- `Academico/AcademicoDbContext.cs` — schema resolvido em runtime por tenant via Finbuckle
  (`tenant_<slug>`): todas as entidades acadêmicas (`Professor`, `Materia`, `Grade`, `Nota`, etc.).

Migrations de cada contexto ficam em `Persistence/Identity/Migrations` e
`Persistence/Academico/Migrations` respectivamente (Code First — ver decisão registrada no
ARCHITECTURE.md e no histórico de conversa: sem `Scaffold-DbContext`, sempre
`dotnet ef migrations add`).

**Design-time factories** (`IdentityDbContextFactory`, `AcademicoDbContextFactory`): usadas só
por `dotnet ef` para gerar/aplicar migrations fora de uma requisição HTTP real — não participam
do runtime da aplicação. `AcademicoDbContextFactory` gera a migration contra o schema fixo
`"template"` (nunca contra um tenant real); em deploy, a mesma migration é aplicada a cada schema
de tenant existente passando o nome do schema como argumento (`-- tenant_petropolis`).

**Cuidado ao filtrar `ApplyConfigurationsFromAssembly`**: os dois `DbContext` compartilham o
mesmo assembly (`FaeterjAcademico.Infrastructure`). Cada um **precisa** filtrar por namespace
(`.Persistence.Identity.Configurations` / `.Persistence.Academico.Configurations`) — sem esse
filtro, `IdentityDbContext` varre e aplica também as configurações acadêmicas, colocando
`Alunos`, `Turmas`, `Notas` etc. dentro do schema `identity` (bug real encontrado e corrigido
durante a Fase 4, ao inspecionar o SQL gerado por `dotnet ef migrations script` antes de aplicar
contra um banco real).
