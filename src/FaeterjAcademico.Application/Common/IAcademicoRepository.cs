using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Common;

/// <summary>
/// Acesso aos dados acadêmicos (schema do tenant atual — ARCHITECTURE.md §3.4), usado pelos
/// casos de uso de Professores, Matérias, Vínculo Matéria-Professor e Grade. Um repositório só,
/// como <see cref="IIdentityRepository"/> — evita explosão de interfaces para um domínio deste
/// porte (decisão consistente com ARCHITECTURE.md, "evitar over-engineering").
/// </summary>
public interface IAcademicoRepository
{
    // Professores
    Task<IReadOnlyList<Professor>> GetProfessoresAsync(CancellationToken cancellationToken);
    Task<Professor?> GetProfessorByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Professor?> GetProfessorByEmailAsync(string email, CancellationToken cancellationToken);
    Task AddProfessorAsync(Professor professor, CancellationToken cancellationToken);
    void RemoveProfessor(Professor professor);
    Task<bool> ProfessorTemVinculoComMateriaAsync(Guid professorId, CancellationToken cancellationToken);

    /// <summary>Professor alocado em alguma Turma de qualquer Grade (não só a ativa) — precisa
    /// ser checado antes de excluir, senão a FK <c>Turmas.ProfessorId</c> quebra o delete
    /// (era um 500 antes desta checagem existir).</summary>
    Task<bool> ProfessorTemTurmaVinculadaAsync(Guid professorId, CancellationToken cancellationToken);

    Task<Professor?> GetProfessorByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);

    // Matérias
    Task<IReadOnlyList<Materia>> GetMateriasAsync(CancellationToken cancellationToken);
    Task<Materia?> GetMateriaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddMateriaAsync(Materia materia, CancellationToken cancellationToken);
    void RemoveMateria(Materia materia);
    Task<bool> MateriaTemVinculoComProfessorAsync(Guid materiaId, CancellationToken cancellationToken);

    // Vínculo Matéria-Professor
    Task<IReadOnlyList<MateriaProfessor>> GetVinculosAsync(CancellationToken cancellationToken);
    Task<MateriaProfessor?> GetVinculoAsync(Guid materiaId, Guid professorId, CancellationToken cancellationToken);
    Task AddVinculoAsync(MateriaProfessor vinculo, CancellationToken cancellationToken);
    void RemoveVinculo(MateriaProfessor vinculo);

    // Período de aula (catálogo de slots do tenant)
    Task<IReadOnlyList<PeriodoAula>> GetPeriodosAulaAsync(CancellationToken cancellationToken);

    // Grade
    Task<Grade?> GetGradeAtivaAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Grade>> GetGradesAsync(CancellationToken cancellationToken);
    Task<Grade?> GetGradeByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddGradeAsync(Grade grade, CancellationToken cancellationToken);
    void RemoveGrade(Grade grade);

    /// <summary>True se alguma Turma desta Grade já tem Nota ou Presença lançada — bloqueia a
    /// exclusão (Turmas→Notas/Presenças é <c>DeleteBehavior.Restrict</c>, então sem essa checagem
    /// o delete quebraria a FK, igual ao caso de Professor — ver ARCHITECTURE.md §7.6).</summary>
    Task<bool> GradeTemDadosAcademicosLancadosAsync(Guid gradeId, CancellationToken cancellationToken);

    // Turmas (aulas alocadas na grade ativa — "minhas turmas" do Professor)
    Task<IReadOnlyList<Turma>> GetTurmasByProfessorIdAsync(Guid professorId, CancellationToken cancellationToken);
    Task<Turma?> GetTurmaByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Turmas da grade ativa cujo período curricular é o de um aluno — "minhas turmas" do Aluno.</summary>
    Task<IReadOnlyList<Turma>> GetTurmasByPeriodoAsync(int periodoCurricular, CancellationToken cancellationToken);

    // Alunos
    Task<IReadOnlyList<Aluno>> GetAlunosAsync(CancellationToken cancellationToken);
    Task<Aluno?> GetAlunoByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Aluno?> GetAlunoByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
    Task<Aluno?> GetAlunoByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Aluno?> GetAlunoByMatriculaAsync(string matricula, CancellationToken cancellationToken);
    Task AddAlunoAsync(Aluno aluno, CancellationToken cancellationToken);

    /// <summary>
    /// Alunos "matriculados" numa turma — o domínio não tem uma entidade de matrícula por turma
    /// (ANALISE-TCC.md/ARCHITECTURE.md): o curso segue coorte por período curricular fechado, então
    /// "alunos da turma" = alunos ativos no mesmo período curricular da turma.
    /// </summary>
    Task<IReadOnlyList<Aluno>> GetAlunosByPeriodoAsync(int periodoCurricular, CancellationToken cancellationToken);

    // Solicitações
    Task<IReadOnlyList<Solicitacao>> GetSolicitacoesAsync(StatusSolicitacao? status, CancellationToken cancellationToken);
    Task<Solicitacao?> GetSolicitacaoByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Solicitacao>> GetSolicitacoesByAlunoAsync(Guid alunoId, CancellationToken cancellationToken);
    Task AddSolicitacaoAsync(Solicitacao solicitacao, CancellationToken cancellationToken);

    // Notas
    Task<IReadOnlyList<Nota>> GetNotasByTurmaAsync(Guid turmaId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Nota>> GetNotasByAlunoAsync(Guid alunoId, CancellationToken cancellationToken);
    Task<Nota?> GetNotaByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddNotaAsync(Nota nota, CancellationToken cancellationToken);

    // Presenças
    Task<IReadOnlyList<Presenca>> GetPresencasByTurmaEDataAsync(Guid turmaId, DateOnly data, CancellationToken cancellationToken);
    Task<IReadOnlyList<Presenca>> GetPresencasByAlunoAsync(Guid alunoId, CancellationToken cancellationToken);
    Task<Presenca?> GetPresencaAsync(Guid alunoId, Guid turmaId, DateOnly data, CancellationToken cancellationToken);
    Task AddPresencaAsync(Presenca presenca, CancellationToken cancellationToken);

    // Materiais complementares
    Task<IReadOnlyList<MaterialComplementar>> GetMateriaisByTurmaAsync(Guid turmaId, CancellationToken cancellationToken);
    Task AddMaterialComplementarAsync(MaterialComplementar material, CancellationToken cancellationToken);

    // Auditoria
    Task AddLogAsync(LogSistema log, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}
