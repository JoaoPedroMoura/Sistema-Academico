using FaeterjAcademico.Application.Attendance.Dtos;
using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Attendance.RegistrarPresenca;

/// <summary>
/// Caso de uso "Lançamento de presença" do Perfil Professor (ANALISE-TCC.md §2). Upsert: se já
/// existe um registro para (aluno, turma, data), corrige em vez de duplicar (índice único no
/// banco também barraria a duplicata, mas aqui tratamos como correção esperada, não erro).
/// </summary>
public sealed class RegistrarPresencaHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<RegistrarPresencaCommand, IReadOnlyList<PresencaDto>>
{
    public async Task<IReadOnlyList<PresencaDto>> HandleAsync(RegistrarPresencaCommand request, CancellationToken cancellationToken = default)
    {
        var (professor, _) = await ProfessorAuthorization.ResolverProfessorETurmaAsync(
            repository, request.AccountId, request.TurmaId, cancellationToken);

        var alunos = (await repository.GetAlunosAsync(cancellationToken)).ToDictionary(a => a.Id);
        var resultado = new List<Presenca>();

        foreach (var registro in request.Registros)
        {
            if (!alunos.ContainsKey(registro.AlunoId))
            {
                continue;
            }

            var existente = await repository.GetPresencaAsync(registro.AlunoId, request.TurmaId, request.DataAula, cancellationToken);
            if (existente is not null)
            {
                existente.Corrigir(registro.Presente);
                resultado.Add(existente);
            }
            else
            {
                var presenca = new Presenca(registro.AlunoId, request.TurmaId, professor.Id, request.DataAula, registro.Presente);
                await repository.AddPresencaAsync(presenca, cancellationToken);
                resultado.Add(presenca);
            }
        }

        await repository.AddLogAsync(new LogSistema(
            currentUser.AccountId, "Presenca.Registrar", "Turma", request.TurmaId, sucesso: true,
            $"{resultado.Count} registro(s) em {request.DataAula:yyyy-MM-dd}."), cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return [.. resultado.Select(p => new PresencaDto(
            p.Id, p.AlunoId, alunos[p.AlunoId].Nome, p.TurmaId, p.DataAula, p.Presente, p.Justificativa))];
    }
}
