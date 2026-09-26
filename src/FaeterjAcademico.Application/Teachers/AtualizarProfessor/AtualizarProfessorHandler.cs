using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Teachers.Dtos;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Teachers.AtualizarProfessor;

public sealed class AtualizarProfessorHandler(
    IAcademicoRepository repository,
    ICurrentUserAccessor currentUser) : IRequestHandler<AtualizarProfessorCommand, ProfessorDto>
{
    public async Task<ProfessorDto> HandleAsync(AtualizarProfessorCommand request, CancellationToken cancellationToken = default)
    {
        var professor = await repository.GetProfessorByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Professor não encontrado.");

        professor.AtualizarDados(request.Nome, request.Telefone);
        // Campo ausente = mantém o que já está salvo.
        professor.AtualizarDocumentos(request.DadosPessoais ?? professor.DadosPessoais, request.Formacao ?? professor.Formacao);
        if (professor.DadosPessoais.Cpf is { } cpf
            && await repository.GetProfessorByCpfAsync(cpf, cancellationToken) is { } outro && outro.Id != professor.Id)
        {
            throw new UseCaseException("Já existe um professor com este CPF.");
        }

        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Professor.Editar", "Professor", professor.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return ProfessorDto.FromEntity(professor);
    }
}
