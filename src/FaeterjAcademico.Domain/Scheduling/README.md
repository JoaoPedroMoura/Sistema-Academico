# Scheduling — Motor GRASP

Implementação do gerador de grade de horário (Fase 5), isolado como serviço de domínio puro —
sem dependência de EF Core, banco ou HTTP. Testável 100% em memória.

Peças previstas (ver [ARCHITECTURE.md §2.3](../../../ARCHITECTURE.md)):
- `IScheduleGenerator` / `GraspScheduleGenerator` — orquestra construção + busca local.
- `GraspOptions` — iterações, limite de estagnação, α da RCL, pesos da função objetivo, seed.
- `ScheduleGenerationInput` / `ScheduleResult` — modelos de entrada/saída, independentes das
  entidades persistidas (mapeamento fica em `Application`).
- Validador das 4 restrições rígidas + calculadora de custo da restrição flexível (janelas).
