# Features

Um diretório por domínio de negócio, nunca por tipo técnico. Cada feature segue a mesma
convenção interna:

```
schedule/
  components/   # UI específica desta feature (ex. GradeTable, GerarGradeButton)
  hooks/        # hooks TanStack Query (useSchedule, useGenerateSchedule)
  api/          # chamadas HTTP isoladas desta feature (scheduleApi.ts), usa shared/api/httpClient
  types.ts      # tipos compartilhados dentro da feature (idealmente espelhando os DTOs da API)
```

Páginas em `app/` importam e compõem esses componentes — nunca implementam lógica de UI
diretamente. Ver [ARCHITECTURE.md §5.1](../../ARCHITECTURE.md).

Features previstas: `schedule`, `grades`, `attendance`, `requests`, `materials`, `teachers`,
`subjects`, `students`, `users`.
