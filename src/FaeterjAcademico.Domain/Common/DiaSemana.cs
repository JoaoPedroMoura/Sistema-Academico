namespace FaeterjAcademico.Domain.Common;

/// <summary>
/// Dias letivos considerados pela grade (segunda a sábado, conforme ANALISE-TCC.md §3.2).
/// Enum próprio em vez de <see cref="DayOfWeek"/> para deixar explícito que domingo não é um
/// valor válido no domínio de agendamento de aulas.
/// </summary>
public enum DiaSemana
{
    Segunda = 1,
    Terca = 2,
    Quarta = 3,
    Quinta = 4,
    Sexta = 5,
    Sabado = 6,
}
