using FaeterjAcademico.Application.Attendance.ListarMinhasPresencas;
using FaeterjAcademico.Application.Attendance.ListarPresencas;
using FaeterjAcademico.Application.Attendance.RegistrarPresenca;
using FaeterjAcademico.Application.Auth.Login;
using FaeterjAcademico.Application.Auth.Logout;
using FaeterjAcademico.Application.Auth.Refresh;
using FaeterjAcademico.Application.Auth.TrocarSenha;
using FaeterjAcademico.Application.Marks.LancarNota;
using FaeterjAcademico.Application.Marks.ListarMinhasNotas;
using FaeterjAcademico.Application.Marks.ListarNotas;
using FaeterjAcademico.Application.Marks.RevisarNota;
using FaeterjAcademico.Application.Materials.EnviarMaterial;
using FaeterjAcademico.Application.Materials.ListarMateriais;
using FaeterjAcademico.Application.Materials.ListarMeusMateriais;
using FaeterjAcademico.Application.Requests.AbrirSolicitacao;
using FaeterjAcademico.Application.Requests.ListarMinhasSolicitacoes;
using FaeterjAcademico.Application.Requests.ListarSolicitacoes;
using FaeterjAcademico.Application.Requests.TriarSolicitacao;
using FaeterjAcademico.Application.Schedule.ExcluirGrade;
using FaeterjAcademico.Application.Schedule.GerarGrade;
using FaeterjAcademico.Application.Schedule.ListarGrades;
using FaeterjAcademico.Application.Schedule.ObterGradeAtiva;
using FaeterjAcademico.Application.Students.AtualizarAluno;
using FaeterjAcademico.Application.Students.CriarAluno;
using FaeterjAcademico.Application.Students.ListarAlunos;
using FaeterjAcademico.Application.Students.ObterMeuPerfilAluno;
using FaeterjAcademico.Application.Subjects.AtualizarMateria;
using FaeterjAcademico.Application.Subjects.CriarMateria;
using FaeterjAcademico.Application.Subjects.ExcluirMateria;
using FaeterjAcademico.Application.Subjects.ListarMaterias;
using FaeterjAcademico.Application.TeacherSubjects.AdicionarVinculo;
using FaeterjAcademico.Application.TeacherSubjects.ListarVinculos;
using FaeterjAcademico.Application.TeacherSubjects.RemoverVinculo;
using FaeterjAcademico.Application.Teachers.AtualizarProfessor;
using FaeterjAcademico.Application.Teachers.CriarProfessor;
using FaeterjAcademico.Application.Teachers.ExcluirProfessor;
using FaeterjAcademico.Application.Teachers.GerenciarDisponibilidade;
using FaeterjAcademico.Application.Teachers.ListarAlunosDaTurma;
using FaeterjAcademico.Application.Teachers.ListarMinhasTurmas;
using FaeterjAcademico.Application.Teachers.ListarProfessores;
using FaeterjAcademico.Application.Teachers.ObterMeuPerfil;
using FaeterjAcademico.Application.Teachers.ObterProfessor;
using Microsoft.Extensions.DependencyInjection;

namespace FaeterjAcademico.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services) => services
        // Auth
        .AddScoped<LoginHandler>()
        .AddScoped<RefreshTokenHandler>()
        .AddScoped<LogoutHandler>()
        .AddScoped<TrocarSenhaHandler>()
        // Teachers
        .AddScoped<CriarProfessorHandler>()
        .AddScoped<AtualizarProfessorHandler>()
        .AddScoped<ExcluirProfessorHandler>()
        .AddScoped<ListarProfessoresHandler>()
        .AddScoped<ObterProfessorHandler>()
        .AddScoped<AdicionarDisponibilidadeHandler>()
        .AddScoped<RemoverDisponibilidadeHandler>()
        .AddScoped<ObterMeuPerfilHandler>()
        .AddScoped<ListarMinhasTurmasHandler>()
        .AddScoped<ListarAlunosDaTurmaHandler>()
        // Subjects
        .AddScoped<CriarMateriaHandler>()
        .AddScoped<AtualizarMateriaHandler>()
        .AddScoped<ExcluirMateriaHandler>()
        .AddScoped<ListarMateriasHandler>()
        // TeacherSubjects
        .AddScoped<AdicionarVinculoHandler>()
        .AddScoped<RemoverVinculoHandler>()
        .AddScoped<ListarVinculosHandler>()
        // Schedule
        .AddScoped<GerarGradeHandler>()
        .AddScoped<ObterGradeAtivaHandler>()
        .AddScoped<ListarGradesHandler>()
        .AddScoped<ExcluirGradeHandler>()
        // Students
        .AddScoped<CriarAlunoHandler>()
        .AddScoped<AtualizarAlunoHandler>()
        .AddScoped<ListarAlunosHandler>()
        .AddScoped<ObterMeuPerfilAlunoHandler>()
        // Requests
        .AddScoped<AbrirSolicitacaoHandler>()
        .AddScoped<ListarSolicitacoesHandler>()
        .AddScoped<ListarMinhasSolicitacoesHandler>()
        .AddScoped<MarcarEmAnaliseHandler>()
        .AddScoped<AprovarSolicitacaoHandler>()
        .AddScoped<RejeitarSolicitacaoHandler>()
        // Marks (Notas)
        .AddScoped<LancarNotaHandler>()
        .AddScoped<RevisarNotaHandler>()
        .AddScoped<ListarNotasHandler>()
        .AddScoped<ListarMinhasNotasHandler>()
        // Attendance (Presença)
        .AddScoped<RegistrarPresencaHandler>()
        .AddScoped<ListarPresencasHandler>()
        .AddScoped<ListarMinhasPresencasHandler>()
        // Materials
        .AddScoped<EnviarMaterialHandler>()
        .AddScoped<ListarMateriaisHandler>()
        .AddScoped<ListarMeusMateriaisHandler>();
}
