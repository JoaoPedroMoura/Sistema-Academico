namespace FaeterjAcademico.Api.Controllers.Auth;

public sealed record LoginRequest(string Email, string Password, string? TenantSlug);

public sealed record TenantOptionResponse(string Slug, string Nome, string Role);

public sealed record LoginResponse(
    bool RequiresTenantSelection,
    IReadOnlyList<TenantOptionResponse> TenantOptions,
    string? AccessToken,
    DateTime? AccessTokenExpiresAtUtc,
    Guid? AccountId,
    string? Nome,
    string? Email,
    string? TenantSlug,
    string? TenantNome,
    string? Role);
