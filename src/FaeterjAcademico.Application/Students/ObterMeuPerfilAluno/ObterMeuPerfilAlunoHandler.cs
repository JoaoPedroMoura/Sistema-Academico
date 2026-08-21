using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Students.Dtos;

namespace FaeterjAcademico.Application.Students.ObterMeuPerfilAluno;

public sealed class ObterMeuPerfilAlunoHandler(IAcademicoRepository repository)
    : IRequestHandler<ObterMeuPerfilAlunoQuery, AlunoDto>
{
    public async Task<AlunoDto> HandleAsync(ObterMeuPerfilAlunoQuery request, CancellationToken cancellationToken = default)
    {
        var aluno = await repository.GetAlunoByAccountIdAsync(request.AccountId, cancellationToken)
            ?? throw new UseCaseException("Conta não corresponde a um aluno desta unidade.");

        return AlunoDto.FromEntity(aluno);
    }
}
