# Application

Casos de uso, organizados por feature (não por tipo técnico) — um subdiretório por área do
domínio: `Schedule/`, `Teachers/`, `Subjects/`, `Grades/`, `Attendance/`, `Requests/`,
`Materials/`, `Users/`.

Cada feature segue a mesma convenção interna, sem mediator (ver
[ARCHITECTURE.md §2.2](../../ARCHITECTURE.md)):

```
Schedule/
  GerarGrade/
    GerarGradeCommand.cs      # dados de entrada
    GerarGradeHandler.cs      # implementa Common.IRequestHandler<GerarGradeCommand, GradeDto>
    GerarGradeValidator.cs    # FluentValidation, injetado e chamado explicitamente no handler
  ConsultarGrade/
    ConsultarGradeQuery.cs
    ConsultarGradeHandler.cs
  Dtos/
    GradeDto.cs
```

`Common/` guarda o contrato `IRequestHandler<TRequest, TResult>` e abstrações compartilhadas
(ex. `IUnitOfWork`, `ICurrentUser`, `ICurrentTenant`) — as implementações ficam em
`Infrastructure`.
