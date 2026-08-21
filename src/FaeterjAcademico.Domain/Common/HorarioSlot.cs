namespace FaeterjAcademico.Domain.Common;

/// <summary>
/// Um slot de horário na grade semanal: dia + faixa (hora início/fim). Value Object — igualdade
/// por valor, imutável. Base de comparação para as restrições rígidas 1-3 (ANALISE-TCC.md §1):
/// dois slots "colidem" se caem no mesmo dia e as faixas se sobrepõem.
/// </summary>
public sealed record HorarioSlot
{
    public DiaSemana Dia { get; }
    public TimeOnly HoraInicio { get; }
    public TimeOnly HoraFim { get; }

    public HorarioSlot(DiaSemana dia, TimeOnly horaInicio, TimeOnly horaFim)
    {
        if (horaFim <= horaInicio)
        {
            throw new ArgumentException("HoraFim deve ser posterior a HoraInicio.", nameof(horaFim));
        }

        Dia = dia;
        HoraInicio = horaInicio;
        HoraFim = horaFim;
    }

    public bool Colide(HorarioSlot outro) =>
        Dia == outro.Dia && HoraInicio < outro.HoraFim && outro.HoraInicio < HoraFim;

    /// <summary>
    /// Verdadeiro se <paramref name="outro"/> está inteiramente contido neste slot — usado para
    /// checar a restrição rígida 4 (ANALISE-TCC.md §1): um período de aula (slot discreto, ex.
    /// "Segunda 7h-7h50") só é válido para um professor se cair dentro de alguma de suas
    /// disponibilidades (faixa mais ampla, ex. "Segunda 7h-12h30").
    /// </summary>
    public bool Contem(HorarioSlot outro) =>
        Dia == outro.Dia && HoraInicio <= outro.HoraInicio && HoraFim >= outro.HoraFim;

    /// <summary>
    /// Verdadeiro se este slot termina exatamente onde o outro começa (ou vice-versa) no mesmo
    /// dia — usado pela busca local do GRASP para detectar aulas consecutivas vs. "janelas"
    /// (ANALISE-TCC.md §1, restrição flexível).
    /// </summary>
    public bool EhConsecutivoA(HorarioSlot outro) =>
        Dia == outro.Dia && (HoraFim == outro.HoraInicio || outro.HoraFim == HoraInicio);
}
