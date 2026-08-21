# Análise do TCC — Base para Evolução do Sistema Acadêmico

> Fonte: `TCC_João_Pedro_Moura_Ferreira.pdf` — "Sistema para a Gestão de Professores Vinculados
> à Instituição e Montagem da Grade de Horário do Curso de Tecnologia da Informação e
> Comunicação da Faeterj-Petrópolis", João Pedro Moura Ferreira, Faeterj/Petrópolis, fevereiro/2025.
> Orientador: Prof. Leonardo da Silva Gomes.

Este documento resume, a partir da leitura integral do PDF (46 páginas), tudo que precisa ser
**preservado como verdade de domínio** na evolução do sistema: o problema resolvido, as regras
de negócio validadas, o algoritmo GRASP, o modelo de entidades e os casos de uso originais.

---

## 1. O problema: Timetabling Problem (Problema de Quadro de Horário)

O sistema original resolve uma instância do **Problema de Quadro de Horário** (*Timetabling
Problem*), definido na literatura (Cooper & Kingston, 1995; Wren, 1996) como atribuir horários,
professores e disciplinas a um conjunto de "reuniões" (aulas) de forma que nenhum participante
precise comparecer a duas simultaneamente.

- **Classe de complexidade:** NP-Completo. Não existe algoritmo exato conhecido que resolva o
  problema em tempo polinomial para instâncias reais — daí a escolha de uma meta-heurística em
  vez de um método exato (Branch and Bound, Relax and Cut).
- **Tamanho do espaço de busca real do curso** (fórmula de Frangouli et al., 1995):
  `B = (bd × bp × bs)^(bm × bd)`, com `bd`=dias letivos, `bp`=períodos/dia, `bs`=salas,
  `bm`=média de aulas/disciplina. Para o curso de TIC da Faeterj-Petrópolis (31 disciplinas,
  ~3,9 tempos/semana, 6 dias letivos, 7 períodos, 5 salas): **B ≈ 6×7×5)^(3,9×31) = 210¹²¹**.
  Isso justifica sozinho o uso de heurística em vez de enumeração/força bruta.
- **Restrições rígidas vs. flexíveis** (Carvalho, 2011): rígidas inviabilizam a solução se
  violadas; flexíveis apenas medem a qualidade/satisfação de uma solução já viável.

### Regras de negócio a preservar no novo domínio

| # | Regra | Tipo |
|---|-------|------|
| 1 | Um professor não pode lecionar mais de uma disciplina no mesmo horário | Rígida |
| 2 | Uma turma não pode ter mais de uma aula no mesmo horário | Rígida |
| 3 | Uma disciplina não pode ter mais de um professor alocado a ela simultaneamente | Rígida |
| 4 | Professores só podem lecionar em horários previamente cadastrados em sua disponibilidade | Rígida |
| 5 | Alocar aulas de uma mesma disciplina de forma consecutiva, evitando janelas | Flexível |

Estas cinco regras são o contrato de validação que os testes unitários do motor GRASP (fase 5 do
projeto) devem cobrir, incluindo casos de borda (professor sem disponibilidade cadastrada,
disciplina sem professor vinculado, etc.).

O próprio autor aponta, na conclusão, oportunidades de melhoria que **não** foram implementadas no
sistema original e que este novo projeto deve considerar ao evoluir o domínio:
- Restrição de sala disponível e capacidade de alunos por turma (não implementada — pode virar
  restrição rígida adicional no novo motor).
- Melhoria da busca local, possivelmente hibridizando com Busca Tabu, para reduzir janelas e
  "aulas soltas" no mesmo dia.
- Áreas exclusivas para professores e alunos (lançamento de notas, faltas, solicitações) —
  **este é exatamente o escopo novo que estamos construindo**.

---

## 2. Fundamentação teórica: por que GRASP

O TCC avalia três famílias de abordagem antes de escolher GRASP:

1. **Algoritmos exatos** (Branch and Bound, Relax and Cut): garantem solução ótima, mas são
   inviáveis computacionalmente para o tamanho do problema (crescimento exponencial).
2. **Meta-heurísticas de busca local genéricas** (Busca Tabu, Simulated Annealing): partem de uma
   solução inicial e navegam pelo espaço de vizinhança tentando escapar de ótimos locais.
3. **Algoritmos Genéticos**: população de soluções, seleção proporcional à aptidão, crossover e
   mutação — descartado por maior complexidade de implementação/tuning sem ganho claro para este
   porte de problema.

**GRASP (Greedy Randomized Adaptive Search Procedure)** foi escolhido por ser uma meta-heurística
*multi-start* simples, rápida e fácil de combinar com outras técnicas, com bons resultados
publicados em problemas de dificuldade similar (Resende & Ribeiro; Fredo & Brito).

### 2.1 Como o GRASP funciona no sistema (duas fases, repetidas por N iterações)

**Fase 1 — Construção (monta a Lista de Candidatos, LC):**
1. Busca todas as matérias cadastradas → lista `M`.
2. Sorteia uma matéria `Mi` de `M`.
3. Para `Mi`, sorteia um professor `Pj` dentre os professores vinculados a `Mi`
   (via `MateriaProfessor`), gerando uma turma candidata `T` para aquele par (matéria, professor).
4. Sorteia uma disponibilidade `d` de `Pj` (dentre os horários que `Pj` cadastrou como disponível).
5. Tenta inserir a atribuição (matéria, professor, horário) na solução `s` em construção.
6. Repete até esgotar as matérias, formando a Lista Restrita de Candidatos por iteração.

> Nota teórica geral do GRASP (Fredo & Brito): a LRC normalmente é limitada por
> `|LRC| = |LC| × α`, restringindo-se aos melhores candidatos da lista completa — o TCC não detalha
> um critério de "melhor" além do sorteio aleatório dentro dos vínculos válidos, ou seja, a
> implementação original é a variante gulosa-aleatória mais simples (sem função de custo por
> candidato na fase de construção).

**Fase 2 — Busca local:**
1. Valida se a solução `s` gerada é viável (respeita as 4 restrições rígidas).
2. Se viável, mede seu "custo"/tempo de geração.
3. Mantém a melhor solução encontrada entre todas as iterações (`s*`).
4. Repete o processo por `x` iterações definidas a priori (parâmetro de entrada).

**Resultado empírico do TCC:** com **120 iterações** o sistema converge para uma solução que
atende todas as restrições rígidas em tempo de execução aceitável, para o cenário real do curso
(31 disciplinas, 13 professores, 5 períodos, segunda a sábado, 7h00–12h30).

### 2.2 Limitações conhecidas (documentadas pelo próprio autor)

- A busca local é rasa: apenas valida viabilidade + mede tempo, sem otimizar ativamente contra a
  restrição flexível de "aulas consecutivas" — por isso ainda podem ocorrer janelas na grade.
  O motor novo deveria tratar isso como função-objetivo explícita (custo por janela) na fase de
  busca local, e não apenas como resultado incidental do sorteio.
- Não há restrição de sala nem de capacidade de turma — hoje o algoritmo ignora esses dois eixos.

---

## 3. Modelo de domínio original

### 3.1 Entidades (Diagrama de Classes e DER — seções 5.3/5.4 do TCC)

As figuras 5.7 (Diagrama de Classes) e 5.8 (DER) são imagens no PDF; as entidades abaixo foram
reconstituídas a partir delas e cruzadas com os casos de uso e o texto do algoritmo:

| Entidade | Papel no domínio |
|---|---|
| `Professor` | Docente vinculado à instituição; possui disponibilidade de horários. |
| `Materia` (Disciplina) | Componente curricular do curso; pertence a um período. |
| `MateriaProfessor` | Associação N:N entre `Materia` e `Professor` (quem pode lecionar o quê). |
| `PreRequisito` | Relação de precedência entre matérias (matéria X exige matéria Y concluída). |
| `Turma` | Instância de uma matéria alocada a um professor e horário(s) dentro da grade. |
| `PeriodoAula` | Um slot de horário (dia da semana + faixa horária) no qual uma aula pode ocorrer. |
| `Grade` | A grade de horários gerada para um período letivo (agrega várias `Turma`/`PeriodoAula`). |
| `Disponibilidade` | Horários em que um `Professor` está apto a lecionar — hoje só consumida
  internamente pelo GRASP; vira tela self-service para o professor no novo sistema. |
| `Usuario` | Conta de acesso ao sistema (login), hoje restrita ao perfil Admin. |
| `LogSistema` | Auditoria: toda operação de escrita (adicionar/editar/excluir) grava log. |

### 3.2 Curso modelado (dados de referência do TCC)

- 31 disciplinas, distribuídas em **5 períodos**.
- 13 professores.
- Grade semanal: segunda a sábado, 7h00 às 12h30 (7 períodos de aula/dia).
- Média de 3,9 tempos de aula por disciplina/semana.

### 3.3 Padrão de auditoria (recorrente em todos os casos de uso)

Todo caso de uso de escrita (Adicionar/Editar/Excluir) segue o mesmo roteiro: validar → persistir
→ **gravar em Log** → redirecionar para a listagem. Regra implícita a preservar: **nenhuma
operação de escrita deve ser silenciosa** — todas devem gerar entrada em log de auditoria,
inclusive tentativas bloqueadas (ex.: exclusão de professor vinculado a matéria é rejeitada e a
tentativa também é logada).

---

## 4. Casos de uso originais (todos hoje restritos ao perfil Admin)

| UC | Nome | Operações | Regra de negação notável |
|---|---|---|---|
| 1 | Manter Professores | Adicionar, Editar, Excluir, Exibir Detalhes, Pesquisar | Exclusão bloqueada se o professor estiver associado a uma matéria. |
| 2 | Manter Matérias | Adicionar, Excluir, Exibir Detalhes, Pesquisar | Exclusão bloqueada se a matéria estiver associada a um professor. |
| 3 | Manter Matérias do Professor | Adicionar, Excluir, Exibir Detalhes, Pesquisar | Exclusão apenas remove o vínculo (não os registros originais). |
| 4 | Manter Funcionários | Adicionar, Editar, Excluir, Exibir Detalhes, Pesquisar | Exclusão de funcionário também exclui o `Usuario` associado. |
| 6* | Manter Usuários | Exibir Detalhes, Pesquisar | Somente leitura — usuários são criados via cadastro de Professor/Funcionário. |
| 7 | Manter/Gerar Grade de Horário | Gerar nova grade, Visualizar por Período | Se já existe grade, sistema pergunta antes de sobrescrever; toda geração é logada. |

\* O documento original numera de 1 a 4 e depois pula para 6 (o "Caso de Uso 5" não aparece no
texto-fonte — provável lapso de numeração do autor; não há caso de uso perdido em termos de
funcionalidade, pois "Visualizar Grade de Horário" está contido no UC7).

Padrão geral de fluxo de pesquisa (repetido em todos): preencher filtro → sistema valida →
sistema lista o resultado.

---

## 5. Stack e decisões técnicas do sistema original (ponto de partida, a ser substituído)

- **Frontend/UI:** Windows Forms (.NET Framework) — desktop, single-user, sem API.
- **Linguagem:** C#.
- **IDE:** Visual Studio Community 2022.
- **Banco de dados:** Microsoft SQL Server 2017 Express.
- **Perfis de acesso:** apenas Admin. Login existe (`Usuario`) mas não há diferenciação de papéis
  nem área para professor/aluno.

Essas escolhas foram adequadas para uma ferramenta administrativa interna de uso único, mas não
suportam multiusuário concorrente, acesso web, nem os novos perfis (Aluno, Professor, Secretaria)
exigidos pela evolução do sistema — motivando a migração para uma arquitetura web (ver
`ARCHITECTURE.md`).

---

## 6. O que muda vs. o que é preservado na evolução

**Preservar (verdade de domínio, não deve ser reinterpretado):**
- As 4 restrições rígidas e a restrição flexível de aulas consecutivas.
- A semântica de `Disponibilidade` como pré-condição de alocação de um professor.
- O algoritmo GRASP em duas fases (construção via sorteio + busca local com N iterações mantendo
  a melhor solução), como núcleo do motor de geração de grade.
- O padrão de auditoria: toda escrita (e toda tentativa bloqueada) gera log.
- As regras de integridade referencial que hoje bloqueiam exclusões (professor↔matéria).

**Evoluir (motivado pelo próprio TCC e pelo novo objetivo do projeto):**
- De desktop single-user (Admin) para web multiusuário com 4 papéis (Admin, Secretaria,
  Professor, Aluno).
- `Disponibilidade` deixa de ser só interna ao algoritmo e vira tela self-service do professor.
- Novas entidades: `Aluno`, `Nota`, `Presenca`, `Solicitacao`, `MaterialComplementar`, e papéis de
  usuário (`Role`) explícitos.
- Gestão de matérias/grade curricular passa da responsabilidade do Admin para a Secretaria
  (mantendo Admin como superusuário que ainda pode operar tudo).
- Possível extensão do motor GRASP com restrições de sala/capacidade e busca local mais rica
  (ex.: custo explícito por janela), conforme sugerido pelo próprio autor na conclusão — a decidir
  se entra no escopo desta fase ou fica registrada como trabalho futuro no novo `ARCHITECTURE.md`.

---

## 7. Perguntas em aberto para a fase de arquitetura

Estas não bloqueiam a leitura/análise, mas precisam de decisão (documentada em `ARCHITECTURE.md`)
antes ou durante o scaffolding:
- Extensão do motor GRASP com restrição de sala/capacidade: entra nesta fase ou fica como
  trabalho futuro documentado?
- Estratégia de migração de dados: existe uma base SQL Server 2017 real em produção a importar, ou
  o novo banco começa vazio (greenfield)?
