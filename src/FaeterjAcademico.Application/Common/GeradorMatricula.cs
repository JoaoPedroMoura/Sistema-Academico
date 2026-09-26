using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Common;

/// <summary>Matrícula de aluno ou professor: prefixo do semestre (ver <see cref="Aluno.PrefixoMatricula"/>) + 5 dígitos aleatórios, sem repetir na unidade.</summary>
public static class GeradorMatricula
{
    private static readonly TimeZoneInfo FusoBrasilia = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public static async Task<string> GerarAsync(IAcademicoRepository academicoRepository, CancellationToken cancellationToken)
    {
        var hoje = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, FusoBrasilia));
        var prefixo = Aluno.PrefixoMatricula(hoje);

        // ponytail: sorteio com retry; 100k números por semestre, colisão só pesa perto de dezenas de milhares de pessoas.
        for (var tentativa = 0; tentativa < 20; tentativa++)
        {
            var matricula = $"{prefixo}{Random.Shared.Next(0, 100_000):D5}";
            if (!await academicoRepository.MatriculaEmUsoAsync(matricula, cancellationToken))
            {
                return matricula;
            }
        }
        throw new UseCaseException("Não foi possível gerar uma matrícula livre. Tente novamente.");
    }
}
