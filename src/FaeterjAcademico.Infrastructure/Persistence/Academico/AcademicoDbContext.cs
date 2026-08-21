using FaeterjAcademico.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaeterjAcademico.Infrastructure.Persistence.Academico;

/// <summary>
/// Dados acadêmicos, isolados por tenant via <c>search_path</c> da conexão Postgres, não via
/// schema fixo no modelo — ver <see cref="AcademicoDbContextOptions"/> para o porquê: EF Core
/// grava o nome do schema dentro de cada migration no momento em que ela é gerada, então um
/// <c>HasDefaultSchema</c> dinâmico aqui não muda o schema já "congelado" nas migrations
/// existentes (ARCHITECTURE.md §3.4). O roteamento para o schema certo (ex.
/// "tenant_petropolis") acontece na connection string, via <c>Options=-c search_path=...</c>.
/// </summary>
public class AcademicoDbContext(DbContextOptions<AcademicoDbContext> options) : DbContext(options)
{
    public DbSet<Professor> Professores => Set<Professor>();
    public DbSet<Disponibilidade> Disponibilidades => Set<Disponibilidade>();
    public DbSet<Materia> Materias => Set<Materia>();
    public DbSet<MateriaProfessor> MateriaProfessores => Set<MateriaProfessor>();
    public DbSet<PreRequisito> PreRequisitos => Set<PreRequisito>();
    public DbSet<PeriodoAula> PeriodosAula => Set<PeriodoAula>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Nota> Notas => Set<Nota>();
    public DbSet<Presenca> Presencas => Set<Presenca>();
    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();
    public DbSet<MaterialComplementar> MateriaisComplementares => Set<MaterialComplementar>();
    public DbSet<LogSistema> LogsSistema => Set<LogSistema>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AcademicoDbContext).Assembly, type =>
            type.Namespace?.Contains(".Persistence.Academico.Configurations") == true);
        base.OnModelCreating(modelBuilder);
    }
}
