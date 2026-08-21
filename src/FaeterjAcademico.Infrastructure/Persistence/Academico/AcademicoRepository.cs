using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico;

public sealed class AcademicoRepository(AcademicoDbContext db) : IAcademicoRepository
{
    // Professores
    public async Task<IReadOnlyList<Professor>> GetProfessoresAsync(CancellationToken cancellationToken) =>
        await db.Professores.Include(p => p.Disponibilidades).OrderBy(p => p.Nome).ToListAsync(cancellationToken);

    public Task<Professor?> GetProfessorByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Professores.Include(p => p.Disponibilidades).SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Professor?> GetProfessorByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Professores.SingleOrDefaultAsync(p => p.Email == email.Trim().ToLower(), cancellationToken);

    public async Task AddProfessorAsync(Professor professor, CancellationToken cancellationToken) =>
        await db.Professores.AddAsync(professor, cancellationToken);

    public void RemoveProfessor(Professor professor) => db.Professores.Remove(professor);

    public Task<bool> ProfessorTemVinculoComMateriaAsync(Guid professorId, CancellationToken cancellationToken) =>
        db.MateriaProfessores.AnyAsync(v => v.ProfessorId == professorId, cancellationToken);

    public Task<Professor?> GetProfessorByAccountIdAsync(Guid accountId, CancellationToken cancellationToken) =>
        db.Professores.Include(p => p.Disponibilidades).SingleOrDefaultAsync(p => p.AccountId == accountId, cancellationToken);

    // Matérias
    public async Task<IReadOnlyList<Materia>> GetMateriasAsync(CancellationToken cancellationToken) =>
        await db.Materias.OrderBy(m => m.Periodo).ThenBy(m => m.Nome).ToListAsync(cancellationToken);

    public Task<Materia?> GetMateriaByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Materias.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task AddMateriaAsync(Materia materia, CancellationToken cancellationToken) =>
        await db.Materias.AddAsync(materia, cancellationToken);

    public void RemoveMateria(Materia materia) => db.Materias.Remove(materia);

    public Task<bool> MateriaTemVinculoComProfessorAsync(Guid materiaId, CancellationToken cancellationToken) =>
        db.MateriaProfessores.AnyAsync(v => v.MateriaId == materiaId, cancellationToken);

    // Vínculo Matéria-Professor
    public async Task<IReadOnlyList<MateriaProfessor>> GetVinculosAsync(CancellationToken cancellationToken) =>
        await db.MateriaProfessores.ToListAsync(cancellationToken);

    public Task<MateriaProfessor?> GetVinculoAsync(Guid materiaId, Guid professorId, CancellationToken cancellationToken) =>
        db.MateriaProfessores.SingleOrDefaultAsync(v => v.MateriaId == materiaId && v.ProfessorId == professorId, cancellationToken);

    public async Task AddVinculoAsync(MateriaProfessor vinculo, CancellationToken cancellationToken) =>
        await db.MateriaProfessores.AddAsync(vinculo, cancellationToken);

    public void RemoveVinculo(MateriaProfessor vinculo) => db.MateriaProfessores.Remove(vinculo);

    // Período de aula
    public async Task<IReadOnlyList<PeriodoAula>> GetPeriodosAulaAsync(CancellationToken cancellationToken) =>
        await db.PeriodosAula.ToListAsync(cancellationToken);

    // Grade
    public Task<Grade?> GetGradeAtivaAsync(CancellationToken cancellationToken) =>
        db.Grades
            .Include(g => g.Turmas)
            .Where(g => g.Status == GradeStatus.Publicada)
            .OrderByDescending(g => g.GeradoEmUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Grade>> GetGradesAsync(CancellationToken cancellationToken) =>
        await db.Grades.Include(g => g.Turmas).OrderByDescending(g => g.GeradoEmUtc).ToListAsync(cancellationToken);

    public Task<Grade?> GetGradeByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Grades.Include(g => g.Turmas).SingleOrDefaultAsync(g => g.Id == id, cancellationToken);

    public async Task AddGradeAsync(Grade grade, CancellationToken cancellationToken) =>
        await db.Grades.AddAsync(grade, cancellationToken);

    // Turmas — só as da grade publicada mais recente ("minha grade" do professor hoje).
    public async Task<IReadOnlyList<Turma>> GetTurmasByProfessorIdAsync(Guid professorId, CancellationToken cancellationToken)
    {
        var gradeAtiva = await db.Grades
            .Where(g => g.Status == GradeStatus.Publicada)
            .OrderByDescending(g => g.GeradoEmUtc)
            .Select(g => g.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return gradeAtiva == Guid.Empty
            ? []
            : await db.Turmas.Where(t => t.GradeId == gradeAtiva && t.ProfessorId == professorId).ToListAsync(cancellationToken);
    }

    public Task<Turma?> GetTurmaByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Turmas.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Turma>> GetTurmasByPeriodoAsync(int periodoCurricular, CancellationToken cancellationToken)
    {
        var gradeAtiva = await db.Grades
            .Where(g => g.Status == GradeStatus.Publicada)
            .OrderByDescending(g => g.GeradoEmUtc)
            .Select(g => g.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return gradeAtiva == Guid.Empty
            ? []
            : await db.Turmas.Where(t => t.GradeId == gradeAtiva && t.PeriodoCurricular == periodoCurricular).ToListAsync(cancellationToken);
    }

    // Alunos
    public async Task<IReadOnlyList<Aluno>> GetAlunosAsync(CancellationToken cancellationToken) =>
        await db.Alunos.OrderBy(a => a.Nome).ToListAsync(cancellationToken);

    public Task<Aluno?> GetAlunoByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Alunos.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

    public Task<Aluno?> GetAlunoByAccountIdAsync(Guid accountId, CancellationToken cancellationToken) =>
        db.Alunos.SingleOrDefaultAsync(a => a.AccountId == accountId, cancellationToken);

    public Task<Aluno?> GetAlunoByEmailAsync(string email, CancellationToken cancellationToken) =>
        db.Alunos.SingleOrDefaultAsync(a => a.Email == email.Trim().ToLower(), cancellationToken);

    public Task<Aluno?> GetAlunoByMatriculaAsync(string matricula, CancellationToken cancellationToken) =>
        db.Alunos.SingleOrDefaultAsync(a => a.Matricula == matricula.Trim(), cancellationToken);

    public async Task AddAlunoAsync(Aluno aluno, CancellationToken cancellationToken) =>
        await db.Alunos.AddAsync(aluno, cancellationToken);

    public async Task<IReadOnlyList<Aluno>> GetAlunosByPeriodoAsync(int periodoCurricular, CancellationToken cancellationToken) =>
        await db.Alunos.Where(a => a.Ativo && a.PeriodoAtual == periodoCurricular).OrderBy(a => a.Nome).ToListAsync(cancellationToken);

    // Solicitações
    public async Task<IReadOnlyList<Solicitacao>> GetSolicitacoesAsync(StatusSolicitacao? status, CancellationToken cancellationToken)
    {
        var query = db.Solicitacoes.AsQueryable();
        if (status is not null)
        {
            query = query.Where(s => s.Status == status);
        }
        return await query.OrderByDescending(s => s.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public Task<Solicitacao?> GetSolicitacaoByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Solicitacoes.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddSolicitacaoAsync(Solicitacao solicitacao, CancellationToken cancellationToken) =>
        await db.Solicitacoes.AddAsync(solicitacao, cancellationToken);

    public async Task<IReadOnlyList<Solicitacao>> GetSolicitacoesByAlunoAsync(Guid alunoId, CancellationToken cancellationToken) =>
        await db.Solicitacoes.Where(s => s.AlunoId == alunoId).OrderByDescending(s => s.CreatedAtUtc).ToListAsync(cancellationToken);

    // Notas
    public async Task<IReadOnlyList<Nota>> GetNotasByTurmaAsync(Guid turmaId, CancellationToken cancellationToken) =>
        await db.Notas.Where(n => n.TurmaId == turmaId).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Nota>> GetNotasByAlunoAsync(Guid alunoId, CancellationToken cancellationToken) =>
        await db.Notas.Where(n => n.AlunoId == alunoId).ToListAsync(cancellationToken);

    public Task<Nota?> GetNotaByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Notas.SingleOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task AddNotaAsync(Nota nota, CancellationToken cancellationToken) =>
        await db.Notas.AddAsync(nota, cancellationToken);

    // Presenças
    public async Task<IReadOnlyList<Presenca>> GetPresencasByTurmaEDataAsync(Guid turmaId, DateOnly data, CancellationToken cancellationToken) =>
        await db.Presencas.Where(p => p.TurmaId == turmaId && p.DataAula == data).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Presenca>> GetPresencasByAlunoAsync(Guid alunoId, CancellationToken cancellationToken) =>
        await db.Presencas.Where(p => p.AlunoId == alunoId).ToListAsync(cancellationToken);

    public Task<Presenca?> GetPresencaAsync(Guid alunoId, Guid turmaId, DateOnly data, CancellationToken cancellationToken) =>
        db.Presencas.SingleOrDefaultAsync(p => p.AlunoId == alunoId && p.TurmaId == turmaId && p.DataAula == data, cancellationToken);

    public async Task AddPresencaAsync(Presenca presenca, CancellationToken cancellationToken) =>
        await db.Presencas.AddAsync(presenca, cancellationToken);

    // Materiais complementares
    public async Task<IReadOnlyList<MaterialComplementar>> GetMateriaisByTurmaAsync(Guid turmaId, CancellationToken cancellationToken) =>
        await db.MateriaisComplementares.Where(m => m.TurmaId == turmaId).OrderByDescending(m => m.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task AddMaterialComplementarAsync(MaterialComplementar material, CancellationToken cancellationToken) =>
        await db.MateriaisComplementares.AddAsync(material, cancellationToken);

    // Auditoria
    public async Task AddLogAsync(LogSistema log, CancellationToken cancellationToken) =>
        await db.LogsSistema.AddAsync(log, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) => db.SaveChangesAsync(cancellationToken);
}
