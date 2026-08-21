# MultiTenancy

Configuração do Finbuckle.MultiTenant (Fase 4/6): `AppTenantInfo` (implementa `ITenantInfo`,
carrega `Id`, `Identifier` = slug, `Name`, nome do schema Postgres), a store de tenants (lida do
schema `identity` via `IdentityDbContext`), e a estratégia de resolução — subdomínio em produção,
header `X-Tenant-Slug` em desenvolvimento (ver [ARCHITECTURE.md §3.3](../../../ARCHITECTURE.md)).
