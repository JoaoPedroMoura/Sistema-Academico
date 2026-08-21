namespace FaeterjAcademico.Domain.Scheduling;

/// <summary>
/// Parâmetros do motor GRASP — ver ARCHITECTURE.md §2.3 para o porquê de cada um (melhorias de
/// baixo/médio esforço sobre o algoritmo original do TCC).
/// </summary>
public sealed record GraspOptions
{
    /// <summary>Teto de segurança de iterações — valor validado empiricamente no TCC original.</summary>
    public int Iterations { get; init; } = 120;

    /// <summary>Para se a melhor solução não melhorar por esta quantidade de iterações seguidas.</summary>
    public int StagnationLimit { get; init; } = 30;

    /// <summary>Tamanho da Lista Restrita de Candidatos: |RCL| = |LC| × α. 0 = totalmente guloso, 1 = totalmente aleatório.</summary>
    public double Alpha { get; init; } = 0.3;

    /// <summary>Peso de cada "janela" (gap entre aulas de um professor no mesmo dia) na função objetivo.</summary>
    public double JanelaWeight { get; init; } = 1.0;

    /// <summary>Peso de cada "aula isolada" (dia com só 1 aula de uma matéria que tem mais aulas na semana) na função objetivo.</summary>
    public double AulaIsoladaWeight { get; init; } = 1.0;

    /// <summary>Semente do gerador aleatório — fixa garante reprodutibilidade (obrigatório para os testes); null usa uma semente aleatória de verdade.</summary>
    public int? Seed { get; init; }

    /// <summary>Roda as iterações em paralelo (multi-start é embaraçosamente paralelo). Resultado é idêntico com ou sem paralelismo, dado o mesmo Seed.</summary>
    public bool ParallelExecution { get; init; } = true;

    /// <summary>Máximo de tentativas de swap na busca local por iteração, como teto de segurança contra runaway em entradas grandes.</summary>
    public int MaxSwapAttempts { get; init; } = 500;
}
