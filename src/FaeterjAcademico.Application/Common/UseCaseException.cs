namespace FaeterjAcademico.Application.Common;

/// <summary>
/// Erro de caso de uso que a Api deve traduzir para uma resposta HTTP específica (ex.: 401/403),
/// diferente de <see cref="Domain.Common.DomainException"/> (violação de regra de negócio do
/// domínio, tipicamente 422/400) e de uma exceção não tratada (500). Nome deliberadamente não
/// "ApplicationException" — colidiria com <see cref="System.ApplicationException"/> do BCL em
/// todo arquivo que importar este namespace.
/// </summary>
public class UseCaseException(string message) : Exception(message);

/// <summary>Credenciais inválidas ou conta sem acesso — sempre mapeado para HTTP 401.</summary>
public sealed class AuthenticationFailedException(string message) : UseCaseException(message);
