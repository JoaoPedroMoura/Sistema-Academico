namespace FaeterjAcademico.Application.Teachers.Dtos;

public sealed record DisponibilidadeDto(Guid Id, string Dia, string HoraInicio, string HoraFim);

public sealed record ProfessorDto(
    Guid Id,
    string Nome,
    string Email,
    string? Telefone,
    bool Ativo,
    IReadOnlyList<DisponibilidadeDto> Disponibilidades)
{
    public static ProfessorDto FromEntity(Domain.Entities.Professor professor) => new(
        professor.Id,
        professor.Nome,
        professor.Email,
        professor.Telefone,
        professor.Ativo,
        [.. professor.Disponibilidades.Select(d => new DisponibilidadeDto(
            d.Id,
            d.Slot.Dia.ToString(),
            d.Slot.HoraInicio.ToString("HH:mm"),
            d.Slot.HoraFim.ToString("HH:mm")))]);
}

/// <summary>Só retornado na criação — a senha temporária não fica salva em lugar nenhum além do hash.</summary>
public sealed record ProfessorCriadoDto(ProfessorDto Professor, string SenhaTemporaria);

/// <summary>Uma aula do professor na grade ativa (área Professor: base para lançar nota/presença/material).</summary>
public sealed record MinhaTurmaDto(
    Guid Id,
    Guid MateriaId,
    string MateriaNome,
    string Dia,
    string HoraInicio,
    string HoraFim,
    int PeriodoCurricular);

/// <summary>Aluno da coorte de uma turma (ver ARCHITECTURE.md — coorte fechada por período curricular).</summary>
public sealed record AlunoResumoDto(Guid Id, string Nome, string Matricula);
