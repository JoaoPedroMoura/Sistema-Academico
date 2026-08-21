# Sistema Acadêmico Faeterj

Evolução do sistema descrito em [ANALISE-TCC.md](ANALISE-TCC.md) — de uma ferramenta desktop
single-user (Admin) para uma aplicação web multiusuário, multi-tenant, com 4 papéis (Admin,
Secretaria, Professor, Aluno). Decisões de stack e arquitetura em [ARCHITECTURE.md](ARCHITECTURE.md).

## Estrutura

```
sistema academico/
  ANALISE-TCC.md         # regras de negócio, entidades e algoritmo GRASP a preservar
  ARCHITECTURE.md         # decisões de stack e arquitetura
  docker-compose.yml       # PostgreSQL local
  src/                      # solution .NET 10 (Clean Architecture)
    FaeterjAcademico.Domain          # entidades + motor GRASP (sem dependências externas)
    FaeterjAcademico.Application     # casos de uso (Commands/Queries), por feature
    FaeterjAcademico.Infrastructure  # EF Core, Npgsql, multi-tenancy, JWT
    FaeterjAcademico.Api             # Controllers, composição
  tests/
    FaeterjAcademico.Domain.Tests
    FaeterjAcademico.Application.Tests
  apps/web/                  # Next.js 16 (App Router, TS estrito)
```

## Rodando localmente

**Backend:**
```bash
docker compose up -d              # sobe o PostgreSQL local (porta 5432)
dotnet build                      # compila a solution inteira
dotnet test                       # roda os testes
dotnet run --project src/FaeterjAcademico.Api
```

> ⚠️ Esta máquina tem **3 serviços nativos do PostgreSQL do Windows** já instalados e rodando,
> ocupando as portas 5432, 5433 e 5434 (`postgresql-x64-15/16/18`). Por isso o Postgres deste
> projeto usa a **porta 5435** em `docker-compose.yml`. Se `localhost:5435` também colidir na sua
> máquina, mude a porta publicada e ajuste a connection string em `appsettings.json` /
> `FAETERJ_DB_CONNECTION` de acordo. Sintoma de estar batendo no Postgres errado: erro de
> autenticação (`28P01`) mesmo com a senha certa — confirme com
> `Get-NetTCPConnection -LocalPort <porta>` no PowerShell antes de investigar credenciais.

**Migrations** (Code First — nunca `Scaffold-DbContext`; isolamento de tenant via `search_path`
da conexão, não via schema fixo no modelo — ver ARCHITECTURE.md §3.4):
```bash
# Schema de identidade (control-plane, único)
dotnet ef database update --project src/FaeterjAcademico.Infrastructure --context IdentityDbContext

# Schema acadêmico de um tenant específico (mesma migration, reaplicável a qualquer schema)
dotnet ef database update --project src/FaeterjAcademico.Infrastructure --context AcademicoDbContext -- tenant_petropolis
```

**Frontend:**
```bash
cd apps/web
cp .env.local.example .env.local
npm install
npm run dev
```

## Criando o primeiro tenant + conta (necessário para logar)

Ainda não há tela de "criar unidade"/"criar conta" (isso é Fase 7 — hoje só existe via SQL
direto). Depois de aplicar as migrations:

```sql
INSERT INTO identity."Tenants" ("Id","Slug","Nome","Ativo","CreatedAtUtc")
VALUES (gen_random_uuid(), 'petropolis', 'Faeterj-Petrópolis', true, now());

-- Hash de senha via BCrypt (workFactor 12) — gere um com BCrypt.Net-Next ou peça pro Claude.
INSERT INTO identity."Accounts" ("Id","Nome","Email","SenhaHash","Ativo","CreatedAtUtc")
VALUES (gen_random_uuid(), 'Admin Petrópolis', 'admin@petropolis.faeterj.edu.br', '<hash bcrypt>', true, now());

INSERT INTO identity."AccountTenantRoles" ("Id","AccountId","TenantId","Role")
VALUES (gen_random_uuid(), '<AccountId>', '<TenantId>', 1); -- 1 = Admin, 2 = Secretaria, 3 = Professor, 4 = Aluno
```

Depois disso, `POST /api/auth/login` (via frontend em `/login`, ou direto na Api) funciona
normalmente.

## Estado atual

- ✅ Fase 1 — Leitura e análise do TCC ([ANALISE-TCC.md](ANALISE-TCC.md))
- ✅ Fase 2 — Decisão de arquitetura ([ARCHITECTURE.md](ARCHITECTURE.md))
- ✅ Fase 3 — Estrutura inicial do projeto
- ✅ Fase 4 — Modelagem de domínio (entidades + migrations)
- ✅ Fase 5 — Motor GRASP (port + melhorias, ver ARCHITECTURE.md §2.3)
- ✅ Fase 6 — Autenticação e perfis (JWT, roteamento protegido por papel)
- ✅ Fase 7 — Telas por perfil (Admin → Secretaria → Professor → Aluno):
  - ✅ **Admin**: Professores + disponibilidade, Matérias, Vínculo Matéria-Professor, Grade
    (geração via GRASP e visualização).
  - ✅ **Secretaria**: matrícula de Alunos (decisão de escopo — ver ARCHITECTURE.md §7.2),
    triagem de Solicitações (marcar em análise / aprovar / rejeitar), acesso a Matérias/Vínculo/
    Grade (mesmos endpoints do Admin, liberados também pro papel Secretaria).
  - ✅ **Professor**: "minhas turmas" (self-service, resolvido da conta autenticada), lançamento
    de notas e presença (com correção/upsert), upload de material complementar (link externo —
    sem storage de arquivo próprio ainda), disponibilidade self-service (ANALISE-TCC.md §6).
  - ✅ **Aluno**: perfil próprio (matrícula/período), consulta de notas por matéria, consulta de
    presença/faltas, download de materiais complementares das suas turmas, abertura e
    acompanhamento de solicitações (ver ARCHITECTURE.md §7.4).
  - Tudo acima validado de ponta a ponta via browser real, com quatro papéis diferentes logados
    (Admin, Secretaria, Professor, Aluno) sobre os mesmos dados.
- ✅ Fase 8 — Revisão final (build, lint, testes, checklist de qualidade) — ver checklist abaixo.

## Checklist de qualidade final (Fase 8)

- ✅ `dotnet build` na solution inteira: **0 erros, 0 warnings**.
- ✅ `dotnet test`: **42/42 testes passando** (17 Application + 25 Domain).
- ✅ `dotnet list package --vulnerable --include-transitive`: nenhuma dependência vulnerável.
- ✅ Frontend: `npx tsc --noEmit`, `npx eslint .` e `npm run build` — todos limpos, sem erros nem
  warnings.
- ✅ `npm audit`: 0 vulnerabilidades.
- ✅ Sem `TODO`/`FIXME`/`HACK` esquecido no código-fonte (backend `src/` e frontend
  `app/`/`features/`/`lib/`/`shared/`).
- ✅ Segurança validada por teste manual (curl), não só por leitura de código:
  - Requisição sem header de tenant → **403**.
  - Requisição com tenant do JWT diferente do tenant resolvido → **403**.
  - Papel sem permissão numa rota de outro papel (ex.: Aluno em rota Admin-only) → **403**.
- ✅ Fluxo de ponta a ponta validado nos 4 papéis nesta revisão final, cada um logando de novo e
  batendo num endpoint representativo: Admin (`GET /api/professores` → 200), Secretaria
  (`GET /api/alunos` → 200), Professor (`GET /api/professores/me/turmas` → 200), Aluno
  (`GET /api/alunos/me` → 200).
- ✅ Documentação consistente com o código: ANALISE-TCC.md (regras preservadas do TCC original),
  ARCHITECTURE.md (decisões de stack + ADRs por fase, incluindo bugs reais encontrados e como
  foram corrigidos), este README (checklist de fases + instruções de setup local).

### Limitações conhecidas / escopo não coberto (ver ARCHITECTURE.md §8 para a lista completa)

- Sem testes automatizados de Application para as features de Professor/Secretaria/Aluno (Marks,
  Attendance, Materials, Requests, Students) — validadas apenas por teste manual de ponta a ponta.
  Auth/Teachers/Subjects, que vieram nas fases iniciais, têm testes com fakes.
- Upload de material complementar aceita apenas link externo (sem storage de arquivo próprio).
- Motor GRASP não considera sala/capacidade — fiel ao escopo original do TCC, por decisão do
  usuário.
- Ver ARCHITECTURE.md §8 para o restante (Reactive GRASP, Path Relinking, regeneração incremental,
  `PlatformAdmin`, database-per-tenant, etc.) — todos registrados como trabalho futuro deliberado,
  não como pendência esquecida.

### Não foi feito nesta revisão (fora do pedido original)

- Não há repositório Git inicializado neste diretório — nenhum commit foi feito em nenhuma fase.
- Não há pipeline de CI/CD configurado.
- Não há deploy de produção — o projeto roda apenas localmente (`docker-compose` + `dotnet run` +
  `npm run dev`), conforme instruções de "rodando localmente" acima.
