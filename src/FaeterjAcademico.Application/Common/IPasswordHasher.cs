namespace FaeterjAcademico.Application.Common;

/// <summary>Abstrai o algoritmo de hash de senha — implementado com BCrypt em Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
