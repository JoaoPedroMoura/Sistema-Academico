using FaeterjAcademico.Application.Common;

namespace FaeterjAcademico.Application.Tests.Auth;

/// <summary>Fake determinístico — "hash" é só o valor com um prefixo, sem criptografia de verdade.</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password) => $"hashed:{password}";

    public bool Verify(string password, string hash) => hash == $"hashed:{password}";
}
