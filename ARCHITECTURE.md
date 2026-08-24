# ARCHITECTURE.md — Sistema Acadêmico Faeterj (evolução multi-unidade)

> Decisões de stack e arquitetura para a evolução do sistema descrito em [ANALISE-TCC.md](ANALISE-TCC.md),
> de uma ferramenta desktop single-user (Admin) para uma aplicação web multiusuário,
> multi-tenant, com 4 papéis (Admin, Secretaria, Professor, Aluno).
>
> Decisões confirmadas com o usuário em 2026-08-21:
> - Motor GRASP preserva o escopo original do TCC (4 restrições rígidas + consecutividade). Sala
>   e capacidade de turma ficam registradas como trabalho futuro (§8).
> - Banco de dados greenfield (sem base legada para migrar) em **PostgreSQL**.
> - Arquitetura **multi-tenant**: cada unidade (Faeterj-Petrópolis, e futuras unidades) tem suas
>   próprias matérias/grade/professores isolados, com uma base central de contas administrando
>   o acesso entre unidades.

---

## 1. Visão geral

```
┌─────────────────────────────┐        ┌──────────────────────────────────────┐
│   Next.js (App Router, TS)  │  JWT   │        ASP.NET Core Web API (.NET 10) │
│   app por feature/domínio   │◄──────►│   Clean Architecture + GRASP engine   │
└─────────────────────────────┘        └──────────────────────────────────────┘
                                                        │
                                          ┌─────────────┴─────────────┐
                                          │        PostgreSQL         │
                                          │  schema "identity" (control-plane) │
                                          │  schema "tenant_<slug>" por unidade │
                                          └────────────────────────────┘
```

---

## 2. Backend

### 2.1 .NET 10 — Controllers, não Minimal APIs

**Decisão:** ASP.NET Core Web API com **Controllers** (não Minimal APIs).

**Por quê:** o domínio tem ~10+ agregados (Professor, Matéria, Turma, Grade, Nota, Presença,
Solicitação, Material, Disponibilidade, Usuário) e 4 papéis com regras de autorização distintas
por recurso. Controllers dão `[Authorize(Roles=...)]` declarativo, filtros de tenant via
`ActionFilter`, e model binding/validação mais previsíveis para uma API deste porte. Minimal APIs
valem a pena para superfícies pequenas (poucos endpoints); aqui o ganho de "menos boilerplate"
seria pago em organização — contraria o pedido de "convenções claras, evitar over-engineering, mas
também evitar código difícil de navegar depois".

### 2.2 Clean Architecture, 4 camadas

```
src/
  FaeterjAcademico.Domain/          # Entidades, Value Objects, regras de negócio puras, motor GRASP
  FaeterjAcademico.Application/     # Casos de uso (CQRS simples: Commands/Queries + Handlers), DTOs, interfaces de repositório
  FaeterjAcademico.Infrastructure/  # EF Core, repositórios, Npgsql, multi-tenancy, JWT, envio de arquivo/storage
  FaeterjAcademico.Api/             # Controllers, middlewares, autenticação, Swagger, composição (DI)
tests/
  FaeterjAcademico.Domain.Tests/    # Testes do motor GRASP e regras de negócio (xUnit)
  FaeterjAcademico.Application.Tests/
```

- **Domain** não referencia nenhum outro projeto (nem EF Core). O motor GRASP mora aqui como
  `IScheduleGenerator`/`GraspScheduleGenerator`, recebendo listas em memória (matérias, professores,
  vínculos, disponibilidades) e devolvendo uma `Grade` candidata — 100% testável sem banco.
- **Application** implementa casos de uso como classes `Command`/`Query` + `Handler` **sem
  biblioteca de mediator** — apenas convenção de nomes e uma interface simples
  (`IRequestHandler<TRequest, TResult>`) injetada via DI padrão do ASP.NET Core, mapeando 1:1 para
  os casos de uso do TCC (`GerarGradeHandler`, `ManterProfessorHandler`, etc.) e os novos
  (`LancarNotaHandler`, `AbrirSolicitacaoHandler`...). Decisão explícita: **MediatR foi descartado**
  (mudou para licenciamento comercial a partir da v13 — risco desnecessário para um sistema que
  pode virar produto real da instituição); o padrão CQRS simples não precisa de um dispatcher em
  runtime, só da separação de intenção leitura/escrita — reduz uma dependência sem perder a
  organização. Evita over-engineering: sem camada extra de "serviços de domínio" genéricos além
  do necessário.
- **Infrastructure** implementa os repositórios com EF Core + Npgsql, a resolução de tenant, o
  emissor/validador de JWT, e o serviço de log de auditoria (persistindo `LogSistema` — regra
  preservada do TCC: toda escrita, inclusive tentativas bloqueadas, gera log).
- **Api** só compõe: controllers finos, filtros de autorização por papel, middleware de resolução
  de tenant, Swagger/OpenAPI.

### 2.3 Motor GRASP — port fiel do TCC + melhorias de baixo/médio esforço

As 4 restrições rígidas permanecem exatamente como no TCC (nenhuma regra de negócio nova). O que
muda é **como o algoritmo busca a solução** — decisão do usuário em 2026-08-21: incluir nesta fase
as melhorias de baixo e médio esforço identificadas na análise; reactive GRASP, path relinking e
regeneração incremental ficam como trabalho futuro (§8).

**Fase de Construção** (evolui de sorteio puro para guloso-aleatório de verdade):
- Para cada disciplina ainda não alocada, monta a Lista de Candidatos (matéria, professor,
  disponibilidade) e ordena pela heurística **"mais restrita primeiro"**: disciplinas com menos
  combinações válidas restantes entram primeiro na RCL — evita empurrar as mais difíceis de
  encaixar para o fim, quando sobra pouca disponibilidade livre.
- **RCL (Lista Restrita de Candidatos)** com parâmetro `α` configurável (`|RCL| = |LC| × α`),
  como no GRASP clássico — em vez do TCC original, que não filtrava por qualidade.
- **Backtracking simples**: se a disciplina no topo da fila não tem candidato viável na iteração
  atual, ela volta para o fim da fila e é retentada após as demais, em vez de a iteração falhar
  silenciosamente.

**Fase de Busca Local** (evolui de "só validar" para melhorar de fato):
- **Função objetivo explícita**: `custo = peso_janela × nº_janelas + peso_dia_isolado × nº_aulas_isoladas`,
  pesos configuráveis — substitui o "mede tempo" do TCC original por uma métrica de qualidade real.
- **Movimento de melhoria (swap local)**: após a construção, tenta trocar o horário de pares de
  turmas já alocadas quando isso reduz o custo sem violar nenhuma das 4 restrições rígidas —
  hill-climbing simples, parar no primeiro ótimo local ou após um teto de tentativas sem melhora.
- **Critério de parada por estagnação**: interrompe se não houver melhora na melhor solução em
  `K` iterações consecutivas (default 30), com teto de segurança em `N` iterações (default 120,
  igual ao valor validado empiricamente no TCC).

**Execução**:
- **Iterações paralelas** (`Parallel.For`/`Task.WhenAll`): cada iteração do GRASP é independente
  por natureza (multi-start) — paraleliza sem mudar a lógica, só reduz o tempo de resposta.
- **RNG com seed configurável**: obrigatório para reprodutibilidade dos testes unitários e debug
  de uma grade específica.

Interface pública:

```csharp
public interface IScheduleGenerator
{
    ScheduleResult Generate(
        ScheduleGenerationInput input,
        GraspOptions options); // Iterations = 120, StagnationLimit = 30, Alpha = 0.3,
                                // JanelaWeight, AulaIsoladaWeight, Seed?
}
```

Testes unitários obrigatórios (Domain.Tests), um por restrição rígida + cenários de qualidade:
1. Professor nunca é alocado 2x no mesmo slot.
2. Turma nunca recebe 2 aulas no mesmo slot.
3. Disciplina nunca tem 2 professores simultâneos.
4. Nenhuma alocação cai fora da `Disponibilidade` cadastrada do professor.
5. Com dados de exemplo do TCC (31 matérias/13 professores), o algoritmo produz solução viável em
   tempo hábil (teste de regressão de performance, não de otimalidade).
6. O swap de busca local nunca piora o custo da solução nem viola restrição rígida (teste de
   invariante do movimento de melhoria).
7. Com seed fixa, duas execuções produzem exatamente a mesma solução (reprodutibilidade).
8. Critério de estagnação encerra antes do teto quando não há melhora (teste de comportamento do
   critério de parada).

Sala e capacidade de turma **não** entram nas restrições agora (decisão do usuário) — a interface
`ScheduleGenerationInput` deixa um campo `Room?`/`Capacity?` opcional, ignorado pelo validador
atual, para não exigir breaking change quando isso for endereçado (§8).

### 2.4 Banco de dados: PostgreSQL

**Decisão:** PostgreSQL 16, via Npgsql + EF Core (Code First, migrations).

**Por quê:** greenfield sem base legada a migrar (confirmado), então não há motivo para carregar
licenciamento do SQL Server. PostgreSQL tem melhor suporte multi-schema nativo (essencial para a
estratégia de tenant abaixo), é gratuito em qualquer ambiente de hospedagem, e tem tooling maduro
no ecossistema .NET (Npgsql).

---

## 3. Multi-tenancy

**Modelo escolhido: banco único, schema-por-tenant**, com um schema central de identidade.

```
PostgreSQL (uma instância)
├── schema "identity"          ← control-plane: Tenants, Contas de usuário, Papéis, RefreshTokens
├── schema "tenant_petropolis" ← dados acadêmicos da Faeterj-Petrópolis (Professor, Materia, Grade...)
├── schema "tenant_<outraunidade>" ← mesma estrutura, dados isolados
```

### 3.1 Por que schema-por-tenant (e não database-por-tenant nem coluna TenantId compartilhada)

| Opção | Isolamento | Operação | Escolhida? |
|---|---|---|---|
| Coluna `TenantId` em tabelas compartilhadas | Fraco (depende de `WHERE` em toda query) | Simples | Não — risco alto de vazamento de dados entre unidades por um filtro esquecido |
| Database por tenant | Forte | Cara (uma migration run por tenant, conexões separadas) | Não agora — over-engineering para o número de unidades previsto |
| **Schema por tenant (escolhida)** | Forte (isolamento estrutural do Postgres) | Uma migration aplica a N schemas; ainda é uma única instância/conexão-pool | **Sim** |

Isso atende diretamente o pedido: "matérias exclusivas para cada [unidade]" — como cada tenant tem
seu próprio schema com suas próprias tabelas `Materia`, `Turma`, `Grade` etc., não existe
possibilidade estrutural de uma unidade enxergar o currículo de outra.

### 3.2 O "banco de admin" (schema `identity`)

Schema central, único para toda a aplicação, contendo:

- `Tenant` (id, slug, nome da unidade, ativo/inativo, regras específicas — ex. dias/horários
  letivos, se aplicável).
- `Account` — conta de login **global** (email, hash de senha, MFA futuro), independente de tenant.
- `AccountTenantRole` — tabela de associação N:N: uma `Account` pode ter papel (`Admin`,
  `Secretaria`, `Professor`, `Aluno`) em um ou mais `Tenant`. Isso cobre o caso de um Admin
  corporativo que administra múltiplas unidades, e o caso comum de aluno/professor vinculado a
  uma única unidade.
- `RefreshToken`, `LoginAudit` — sessão e auditoria de acesso (separado do `LogSistema` de
  operações de negócio, que vive dentro de cada schema de tenant).

O JWT emitido carrega `sub` (AccountId), `tenant` (slug do tenant ativo na sessão) e `role`
(resolvido a partir de `AccountTenantRole` para aquele tenant). Login exige selecionar/relacionar
o tenant quando a conta tem acesso a mais de um (fluxo igual ao de ferramentas B2B multi-org
comuns — ex. Slack/Vercel).

### 3.3 Resolução de tenant nas requisições

- **Produção:** subdomínio (`petropolis.<dominio>`) resolve o tenant antes de qualquer
  autenticação, validado contra o JWT (o tenant do token deve bater com o tenant do subdomínio).
- **Dev local:** header `X-Tenant-Slug` (evita depender de DNS/hosts local).
- Implementado com **Finbuckle.MultiTenant** (biblioteca madura para ASP.NET Core + EF Core),
  evitando reinventar resolução de tenant e troca de `search_path` do Npgsql na mão — reduz
  código customizado a manter.

### 3.4 Migrations

- Um `DbContext` para o schema `identity` (`IdentityDbContext`), schema fixo, migrations próprias
  normais.
- Um `DbContext` para dados de tenant (`AcademicoDbContext`) — **isolamento via `search_path` da
  conexão Postgres, não via schema fixo no modelo.** Isso foi uma correção de design feita ao
  aplicar as migrations de fato pela primeira vez (Fase 4): o EF Core grava o nome do schema
  *dentro de cada arquivo de migration* no momento em que ela é gerada (`CreateTable(schema:
  "x", ...)`) — um `HasDefaultSchema` dinâmico no `OnModelCreating` não muda esse valor já
  "congelado" nas migrations existentes, então a mesma migration aplicada a dois tenants
  diferentes acabava sempre escrevendo no mesmo schema (o do momento em que foi gerada). A
  correção: o modelo do `AcademicoDbContext` não declara schema nenhum (todo SQL gerado é
  schema-agnóstico); o roteamento para o schema certo acontece na connection string, via
  `Options=-c search_path=tenant_petropolis` — padrão recomendado pela comunidade EF Core para
  schema-per-tenant. `AcademicoDbContextOptions.Configure(...)` centraliza isso (usado tanto pela
  design-time factory quanto pelo registro de DI em runtime — Fase 6) e também garante que o
  schema existe (`CREATE SCHEMA IF NOT EXISTS`) antes de migrar, já que `search_path` aponta para
  um schema mas não o cria. Validado de ponta a ponta: mesma migration aplicada a
  `tenant_petropolis` e a um tenant de teste, isolamento de dados confirmado por inserção direta.
- As migrations são geradas uma vez (contra um schema "template" descartável, só para o design
  time ter algo pra apontar) e aplicadas a cada schema de tenant existente por um job de deploy
  (`dotnet ef database update -- <schema>` iterado pela lista de tenants ativos, ou migration
  runner customizado simples — sem necessidade de ferramenta externa para o número de tenants
  previsto).

---

## 4. Autenticação e autorização

- **JWT** (access token curto, ~15min + refresh token rotativo, httpOnly cookie — ver §5.2 para
  onde esse cookie efetivamente vive, via BFF do Next.js).
- Papéis: `Admin`, `Secretaria`, `Professor`, `Aluno` — via claim `role` (`ClaimTypes.Role`),
  checados com `[Authorize(Roles = "...")]` nos controllers e com `RequireRole` nas rotas
  protegidas do Next.js (§5.2).
- Admin é tratado como superusuário **dentro do seu(s) tenant(s)** — não existe um "super-admin
  cross-tenant" implícito; se necessário no futuro, isso vira um papel adicional explícito
  (`PlatformAdmin`) no schema `identity`, não uma flag escondida.
- Claims do access token: `sub` (AccountId), `email`, `name`, `tenant` (slug), `tenant_name`,
  `role`. Refresh token é vinculado a um `TenantId` específico (não só à conta) — refresh sempre
  reemite para a mesma unidade da sessão original; trocar de unidade exige logout + login de
  novo, nunca é silencioso.
- **Trava de tenant (encontrada e fechada na Fase 6, `Program.cs` da Api):** `[Authorize(Roles =
  ...)]` sozinho não impede que um JWT válido de uma unidade seja usado contra outra — papel e
  tenant são checados por mecanismos independentes (claims do token vs. resolução Finbuckle da
  requisição). Um middleware dedicado, rodando logo após `UseAuthentication()`, compara o claim
  `tenant` do token com o tenant resolvido pelo Finbuckle para aquela requisição (header/
  subdomínio) e devolve 403 se não baterem — **inclusive quando nenhum tenant foi resolvido**
  (header ausente ou apontando pra um slug inexistente; a primeira versão dessa trava só
  bloqueava "resolveu para outro tenant", deixando passar "não resolveu nenhum" — corrigido e
  testado com os três casos: mesmo tenant, tenant inexistente, header ausente).

---

## 5. Frontend

### 5.1 Next.js — App Router, TypeScript estrito

```
apps/web/
  app/
    (auth)/login/                     # rotas públicas
    (app)/admin/...                   # rotas protegidas por papel (route groups)
    (app)/secretaria/...
    (app)/professor/...
    (app)/aluno/...
    layout.tsx, providers.tsx         # composição global (TanStack Query, tema)
  features/
    schedule/        # geração/visualização de grade (GRASP)
      components/    # componentes de UI específicos da feature
      hooks/         # useSchedule, useGenerateSchedule...
      api/           # camada de chamadas HTTP isoladas (scheduleApi.ts)
      types.ts
    grades/           # notas (Nota)
    attendance/        # presença/faltas
    requests/           # solicitações (aluno ↔ secretaria)
    materials/           # materiais complementares
    teachers/, subjects/, students/, users/   # cadastros (Admin/Secretaria)
  shared/
    components/       # UI genérica reutilizável (Button, Table, Modal, DataGrid...)
    hooks/
    api/               # cliente HTTP base (fetch wrapper com auth), tipos gerados da API
    styles/            # tokens de design (ver §5.3)
  lib/
    auth/              # contexto de sessão, guarda de rota por papel
```

- **Regra não negociável:** nenhuma página em `app/` deve conter lógica de UI complexa — páginas
  compõem componentes de `features/*` e `shared/components`. Página = orquestração, não implementação.
- **Server Components** para leitura inicial (listagens, detalhes) — busca direto na API a partir
  do servidor. **Client Components** só onde há interatividade (formulários, geração de grade com
  progresso, upload de material).
- **TanStack Query** para todo dado de servidor no client-side (cache, invalidação após
  mutations, estados de loading/erro padronizados via hooks de `features/*`).

### 5.2 Autorização no frontend

**Next.js 16 renomeou `middleware.ts` para `proxy.ts`** (o próprio template já vem com um
`AGENTS.md` avisando disso — convenção nova, comportamento equivalente). Route groups por papel
(`(app)/admin`, `(app)/professor`, etc.) com `proxy.ts` na raiz verificando a sessão antes de
renderizar — evita flash de conteúdo não autorizado e centraliza a regra em um único lugar em vez
de checagem espalhada por página.

**Padrão BFF para o refresh token (decisão da Fase 6):** o refresh token vive num cookie
httpOnly, mas a API .NET e o Next.js rodam em origens diferentes mesmo em dev
(`localhost:5100` vs `localhost:3000`). Um cookie setado diretamente pela API ficaria preso à
origem dela — o `proxy.ts`, rodando no servidor do Next, nunca o veria (cookies de origens
diferentes não aparecem em `request.cookies` de outra origem). A solução: `apps/web/app/api/auth/
{login,refresh,logout}/route.ts` são rotas do próprio Next.js que fazem proxy para a API .NET,
capturam o `Set-Cookie` da resposta e o reemitem na origem do Next (`Path=/`, não mais
`Path=/api/auth` como a API usa internamente) — assim o `proxy.ts` consegue ler o cookie em
qualquer rota. O frontend nunca fala com `/api/auth/*` da API .NET diretamente; sempre com as
rotas locais espelhadas. Chamadas de negócio (Fase 7) continuam indo direto na API .NET com o
access token no header `Authorization`, sem passar pelo BFF — só o fluxo baseado em cookie precisa
dele.

Duas camadas de proteção, cada uma no nível certo (`apps/web/proxy.ts` +
`shared/components/RequireRole.tsx`): o `proxy.ts` só checa a *presença* do cookie (rápido, roda em
toda requisição, redireciona pra `/login` sem sessão nenhuma); `RequireRole`, no layout de cada
área, restaura a sessão completa via `POST /api/auth/refresh` (`lib/auth/useSessionBootstrap.ts`)
e redireciona para a área certa se o papel da sessão não bate com a área acessada (ex. Professor
tentando abrir `/admin`).

### 5.3 Integração com Design Tokens (Claude Design, sessão separada)

**Tailwind v4 é CSS-first** (o `create-next-app` atual não gera `tailwind.config.ts` — tema é
declarado via `@theme` dentro do CSS). Estrutura adaptada a isso, mas com o mesmo objetivo (trocar
um arquivo re-estiliza tudo, sem tocar em componentes):

- `shared/styles/tokens.css` — variáveis CSS (`--color-*`, `--font-*`, `--space-*`, `--radius-*`)
  como placeholder neutro por enquanto; será substituído pelo output da sessão de design.
- `app/globals.css` importa `tokens.css` e mapeia cada variável para o namespace do Tailwind via
  `@theme inline { --color-primary: var(--color-primary); ... }` — isso gera as classes utilitárias
  (`bg-primary`, `text-muted-foreground`, etc.) automaticamente a partir dos tokens.
- Componentes de `shared/components` não usam cor/espaçamento literal, sempre via essas classes ou
  `var(--color-*)` diretamente — garante que o replace futuro não exija tocar em componentes.
- **Validação de formulário:** `zod` (já instalado) para schemas compartilháveis entre validação de
  formulário no client e tipagem dos DTOs — evita duplicar regra de "campo obrigatório" em dois
  lugares.

---

## 6. O que **não** estamos fazendo agora (evitar over-engineering)

- Sem microserviços — API única modular é suficiente para o porte do domínio.
- Sem message broker/event bus — operações são request/response diretas; auditoria é gravação
  síncrona no mesmo `DbContext` transacional do caso de uso.
- Sem GraphQL — REST simples via Controllers atende; menor superfície de aprendizado.
- Sem database-per-tenant — adiado até (se) houver requisito real de isolamento físico/compliance
  por unidade.
- Sem CQRS com Event Sourcing — "CQRS simples" (Commands/Queries via MediatR) apenas para separar
  intenção de leitura/escrita, sem sourcing de eventos.

---

## 7. Estrutura de pastas do repositório (nível raiz)

```
sistema academico/
  ANALISE-TCC.md
  ARCHITECTURE.md
  src/                     # solution .NET (backend)
  tests/
  apps/web/                # Next.js (frontend)
  TCC_João_Pedro_Moura_Ferreira.pdf
```

---

## 7.1 Bug real do EF Core encontrado na Fase 7: chaves Guid geradas no cliente

Toda entidade do domínio gera seu próprio `Id` (`Guid.NewGuid()`) no construtor, nunca deixando o
banco gerar a chave. Isso quebrou silenciosamente ao adicionar um filho novo a uma coleção
navegada já rastreada (ex. `professor.AdicionarDisponibilidade(slot)` sobre um `Professor` já
carregado do banco): o EF Core, por convenção, trata `Guid` como `ValueGeneratedOnAdd` — e como o
`Id` já vem preenchido (não é `Guid.Empty`) no momento em que a entidade nova aparece via
`DetectChanges` (não via `Add()` explícito), o EF não consegue decidir se é uma entidade nova ou
uma já existente sendo re-anexada, e às vezes assume "já existe" e gera um `UPDATE` em vez de um
`INSERT` — que afeta 0 linhas (`DbUpdateConcurrencyException`) porque a linha nunca existiu.

**Correção:** `builder.Property(x => x.Id).ValueGeneratedNever()` em **todas as 19 configurações
EF Core** (Identity + Academico) — já que 100% das entidades geram sua própria chave, isso remove
a ambiguidade de vez. Aplicado sistemicamente, não caso a caso. Só foi encontrado porque o fluxo
de "adicionar disponibilidade" foi testado de verdade contra o Postgres via API real (retornava
500) — não apareceria em testes unitários de domínio (que não tocam EF Core).

## 7.2 Decisão de escopo da Fase 7: matrícula de Alunos é responsabilidade da Secretaria

O prompt original não listava "Manter Alunos" explicitamente para nenhum papel — só "Triagem de
solicitações", que pressupõe que alunos já existam. Sem alguém pra matricular aluno, a área da
Secretaria não teria o que triar de verdade. Decisão: matrícula (criar conta + registro de
Aluno) fica com a Secretaria, seguindo o mesmo padrão de "Admin cria Professor" (conta + entidade
+ papel, numa operação só — `CriarAlunoHandler`, mesma ressalva de consistência entre schemas do
§7.1/CriarProfessorHandler). Justificativa prática: em instituições brasileiras, matrícula é
tipicamente front-office/secretaria, não TI (Admin) nem autoatendimento do próprio aluno.

Também abri `AbrirSolicitacaoHandler` (Perfil Aluno) nesta fase, não na fase do Aluno — sem
isso, não haveria solicitação nenhuma pra Secretaria triar de ponta a ponta. O `AlunoId` é sempre
resolvido a partir da `AccountId` do JWT autenticado (`GetAlunoByAccountIdAsync`), nunca aceito
do cliente — evita um aluno abrir solicitação em nome de outro.

`SolicitacoesController` não tem `[Authorize]` de classe — cada ação declara seu papel
(`Aluno` abre, `Admin,Secretaria` triam) porque são mutuamente exclusivos; um `[Authorize]` de
classe combinado com outro por ação vira AND, não OR, e ninguém passaria.

## 7.3 Área do Professor: coorte por período, não matrícula por turma

O domínio (ANALISE-TCC.md/§5.4 do TCC original) nunca teve uma entidade de "matrícula por
turma" — o curso segue coorte fechada por período curricular (todo aluno do 1º período faz as
mesmas matérias do 1º período). Decisão desta fase: "alunos de uma turma" = alunos ativos com
`PeriodoAtual == Turma.PeriodoCurricular`, calculado em `GetAlunosByPeriodoAsync` — sem entidade
nova. Validado na prática: ao testar de ponta a ponta, avancei a aluna de teste do 1º pro 2º
período (Fase da Secretaria) e, ao abrir a turma de Algoritmos (1º período) na área do Professor,
ela **corretamente** não aparecia mais na lista — a nota já lançada continua no histórico, mas
ela não é mais "aluna da turma" daquele período. Comportamento correto do modelo, não bug —
descoberto exatamente pelo tipo de teste de ponta a ponta que este projeto vem fazendo em cada
fase.

`ProfessorAuthorization.ResolverProfessorETurmaAsync` (`Application/Common`) centraliza "essa
turma é do professor autenticado?" — usado por Notas, Presença e Materiais, os três só aceitam
escrita numa turma que pertence ao professor da sessão (testado: um professor tentando lançar
nota na turma de outro recebe 400).

Presença suporta correção (upsert): lançar de novo para o mesmo aluno/turma/data atualiza em vez
de duplicar — adicionei `Presenca.Corrigir(bool)` ao domínio pra isso.

Upload de material complementar **não tem armazenamento de arquivo próprio** (sem S3/Azure Blob
nesta fase) — o professor cola um link de um arquivo já hospedado em outro lugar. Registrado como
trabalho futuro (§8); a Api não finge um upload que não existe.

## 7.4 Área do Aluno: consultas somente-leitura + solicitações

Última área da Fase 7. Todos os endpoints resolvem o aluno a partir da conta autenticada
(`GetAlunoByAccountIdAsync`/`ObterMeuPerfilAlunoHandler`), nunca de um id vindo do cliente — mesmo
padrão de segurança já usado no self-service do Professor:

- `GET /api/alunos/me` — perfil (nome, matrícula, período atual).
- `GET /api/notas/minhas` — notas do aluno em todas as turmas, com `MateriaNome` já resolvido
  (`MinhaNotaDto`), pra não precisar de outra chamada pra exibir por matéria.
- `GET /api/presencas/minhas` — idem para presença (`MinhaPresencaDto`, inclui `Justificativa`).
- `GET /api/materiais/meus` — materiais complementares das turmas em que o aluno está matriculado
  no período atual (reusa a mesma noção de coorte por período do §7.3), com `MateriaNome`
  (`MeuMaterialDto`).
- `GET /api/solicitacoes/minhas` + `POST /api/solicitacoes` (já existente, reaproveitado) — abrir e
  acompanhar status/resposta das próprias solicitações.

Duas queries novas no repositório sustentam isso: `GetTurmasByPeriodoAsync` (turmas da grade ativa
para um período curricular — usada para resolver "materiais das minhas turmas" sem entidade de
matrícula) e `GetSolicitacoesByAlunoAsync`.

Frontend: cada feature já existente (`features/{grades,attendance,materials,requests}`) ganhou o
par tipo/api/hook "minha(s)" ao lado do que já existia para Professor/Secretaria, em vez de criar
features novas — o recurso de domínio é o mesmo, só a visão muda. `app/(app)/aluno/` deixou de ser
o placeholder da Fase 3 e ganhou uma home com cards de navegação + perfil, e uma página por
subárea (`notas`, `presencas`, `materiais`, `solicitacoes`). Validado de ponta a ponta: build/lint/
typecheck limpos, `dotnet test` com os mesmos 42 testes passando (nenhum teste novo de Application
para esta fase — mesma lacuna já registrada no §8 para o Professor), e fluxo real no browser login
→ consulta de notas/presença/materiais → abertura de uma nova solicitação → aparecendo
imediatamente na lista com status "Aberta".

## 7.5 Troca obrigatória de senha temporária

Toda conta criada por Admin/Secretaria (Professor, Aluno) nasce com uma senha temporária
(`TemporaryPasswordGenerator`) — até aqui, nada impedia o usuário de continuar usando ela para
sempre. Adicionado: `Account.DeveTrocarSenha` (schema `identity`, migration
`AddDeveTrocarSenhaToAccount`), `true` só quando a conta é criada via `Account(..., senhaTemporaria:
true)` — contas semeadas direto por SQL (Admin/Secretaria iniciais) e as já existentes no banco
antes desta migration ficam `false` (default explícito na coluna).

**Decisão de escopo — enforcement é client-side, não um novo boundary de segurança no backend:**
`LoginResult`/`RefreshTokenResult` passam `PrecisaTrocarSenha` pro frontend, que redireciona pra
`/trocar-senha` antes de liberar qualquer área (`RequireRole` checa a flag antes até do papel).
O backend **não** bloqueia os outros endpoints enquanto a flag é true — só oferece
`POST /api/me/trocar-senha` (exige a senha atual, autenticado, sem restrição de Role) que a limpa.
Coerente com o resto do projeto: `RequireRole` já é documentado como UX, não boundary (o boundary
real são `proxy.ts` + `[Authorize(Roles=...)]` por endpoint) — "ainda não trocou a senha temporária"
não é uma questão de autorização entre usuários, então não justifica um filtro global em todo
Controller. Se um usuário decidir ignorar o redirect e chamar outro endpoint manualmente com o
token, ele consegue — mas só nos dados que o próprio papel dele já tem acesso.

Validado de ponta a ponta com um professor de teste criado via Admin: login com a senha temporária
devolve `precisaTrocarSenha: true` → browser real redireciona pra `/trocar-senha` → depois de
trocar, cai na área do Professor normalmente → login de novo com a senha antiga dá 401, com a nova
devolve `precisaTrocarSenha: false`.

## 7.6 Bug real: exclusão de Professor alocado em Turma quebrava com 500

Reportado pelo usuário testando via browser: `DELETE /api/professores/{id}` de um professor que
já tinha sido alocado numa Turma por uma Grade gerada (mesmo uma Grade antiga/não-ativa) estourava
`DbUpdateException` — FK `FK_Turmas_Professores_ProfessorId` — e virava 500. A checagem de bloqueio
existente (`ProfessorTemVinculoComMateriaAsync`) só cobria o vínculo com Matéria, não a alocação em
Turma.

Corrigido com o mesmo padrão já usado para o caso de Matéria: nova
`ProfessorTemTurmaVinculadaAsync` (checa `Turmas.ProfessorId` em qualquer Grade, não só a ativa —
uma Grade arquivada ainda tem a FK) em `ExcluirProfessorHandler`, bloqueando antes do delete com
uma `UseCaseException` (→ 400, mensagem clara) em vez de deixar o Postgres estourar a constraint
(→ 500 genérico). Tentativa bloqueada também vira `LogSistema`, mesmo padrão do caso de Matéria.

Regra que não existia no TCC original (lá a exclusão era single-user, sem o conceito de grade já
publicada persistir depois — ANALISE-TCC.md). Validado via curl reproduzindo o request exato do
bug report (professor com turma alocada → 400 com mensagem) e um caso de regressão (professor sem
nenhum vínculo → 204 normal).

## 7.7 Exclusão de Grade + navegação "voltar" nas subpáginas

Duas melhorias de UX pedidas pelo usuário depois de testar a Fase 7 no browser:

**Excluir grade** — não existia no TCC original (lá só se gerava grade, nunca se descartava uma).
`DELETE /api/grades/{id}` (`ExcluirGradeHandler`), mesmo padrão de guarda que o bug do §7.6: bloqueia
se qualquer Turma da grade já tem Nota ou Presença lançada (`GradeTemDadosAcademicosLancadosAsync`
— essas FKs também são `Restrict`, sem a checagem o delete quebraria do mesmo jeito). Materiais
complementares são `Cascade` e somem junto — são só links, não histórico. Botão "Excluir grade"
aparece em `admin/grade` e `secretaria/grade` só quando há uma grade ativa pra excluir, com
`window.confirm` antes (única confirmação desse tipo no projeto — justificada por ser uma exclusão
estrutural, não uma linha de tabela como Professor/Matéria). Validado via curl: grade com dados
acadêmicos → 400 bloqueado; grade recém-gerada sem dados → 204.

**Navegação "voltar"** — `professor/disponibilidade`, `professor/turmas/[id]` e as subpáginas do
Aluno já tinham um link "← [Área]" pro menu principal do papel; `admin/professores`,
`admin/materias`, `admin/grade`, `secretaria/alunos`, `secretaria/materias`, `secretaria/grade` e
`secretaria/solicitacoes` não tinham. Centralizado num componente `BackLink`
(`shared/components/BackLink.tsx`) e adicionado nas 7 páginas que faltavam.

## 8. Trabalho futuro (fora do escopo desta fase, registrado por decisão do usuário)

- Restrição de sala disponível e capacidade de alunos por turma no motor GRASP.
- **Reactive GRASP**: ajuste automático do parâmetro `α` da RCL com base na qualidade das
  soluções recentes, em vez de fixo.
- **Path Relinking** entre um pool de soluções elite (top 5), combinando boas sub-soluções em vez
  de manter apenas a melhor.
- **Regeneração incremental**: quando só a disponibilidade de um professor muda, regenerar apenas
  as turmas afetadas em vez de rodar o GRASP completo de novo.
- Possível hibridização da busca local com Busca Tabu, conforme sugerido pelo autor do TCC, caso
  o hill-climbing simples (swap) não seja suficiente na prática.
- Papel `PlatformAdmin` cross-tenant explícito, caso surja necessidade operacional de administrar
  todas as unidades a partir de uma única conta.
- Database-per-tenant, caso alguma unidade exija isolamento físico por motivo de compliance.
- **Armazenamento de arquivo próprio** (S3/Azure Blob) para materiais complementares — hoje o
  professor só cola um link externo (§7.3).
- Testes unitários de Application para as features da Fase do Professor e do Aluno (Marks,
  Attendance, Materials, Requests, Students) — validadas via teste manual de ponta a ponta (curl +
  browser), mas sem os testes automatizados com fakes que as features de Auth/Teachers/Subjects já
  têm.

---

## 9. Status final (Fase 8 — Revisão final)

Todas as 8 fases planejadas foram entregues: estrutura inicial, modelagem de domínio, motor GRASP,
autenticação, e as 4 áreas por papel (Admin, Secretaria, Professor, Aluno). O checklist de
qualidade da revisão final (build/lint/testes/segurança, mais as limitações conhecidas e o que
ficou deliberadamente fora do escopo) está no [README.md](README.md), seção "Checklist de
qualidade final (Fase 8)" — para não duplicar o mesmo conteúdo em dois documentos. Trabalho futuro
além do escopo desta fase está listado em §8 acima.
