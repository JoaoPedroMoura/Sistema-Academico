namespace FaeterjAcademico.Application.Common;

/// <summary>
/// Contrato simples de caso de uso (Command ou Query), sem mediator/dispatcher.
/// Cada handler é registrado e injetado diretamente onde é consumido (normalmente um Controller).
/// </summary>
public interface IRequestHandler<in TRequest, TResult>
{
    Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// Variante para casos de uso que não retornam dado (comandos puros de efeito colateral).
/// </summary>
public interface IRequestHandler<in TRequest>
{
    Task HandleAsync(TRequest request, CancellationToken cancellationToken = default);
}
