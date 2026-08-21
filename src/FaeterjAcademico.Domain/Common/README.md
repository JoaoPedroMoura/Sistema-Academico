# Common

Tipos-base compartilhados por todo o domínio: classe base de entidade (Id, timestamps de
auditoria), Value Objects (ex. `HorarioSlot` = dia da semana + faixa horária), e exceções de
domínio (`DomainException`, `RegraDeNegocioVioladaException`). Sem dependência externa.
