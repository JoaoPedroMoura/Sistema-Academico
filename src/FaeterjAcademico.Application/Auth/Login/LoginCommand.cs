namespace FaeterjAcademico.Application.Auth.Login;

/// <summary>
/// <paramref name="TenantSlug"/> é obrigatório só quando a conta tem acesso a mais de uma
/// unidade — ver <see cref="LoginResult"/> (ARCHITECTURE.md §3.2/§4).
/// </summary>
public sealed record LoginCommand(string Email, string Password, string? TenantSlug, string? IpAddress);
