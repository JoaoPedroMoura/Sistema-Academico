using System.Security.Cryptography;

namespace FaeterjAcademico.Application.Common;

/// <summary>Gera senhas temporárias para contas criadas por Admin/Secretaria (professor, aluno).</summary>
public static class TemporaryPasswordGenerator
{
    private const string Alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";

    public static string Gerar(int tamanho = 12)
    {
        Span<char> chars = tamanho <= 256 ? stackalloc char[tamanho] : new char[tamanho];
        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = Alfabeto[RandomNumberGenerator.GetInt32(Alfabeto.Length)];
        }
        return new string(chars);
    }
}
