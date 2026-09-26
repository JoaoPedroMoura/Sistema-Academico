using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Domain.Entities;

namespace FaeterjAcademico.Application.Documents;

/// <summary>Metadados do arquivo — o conteúdo só sai pelo download.</summary>
public sealed record DocumentoAnexoDto(
    Guid Id, TipoDocumento Tipo, string NomeArquivo, string ContentType, long TamanhoBytes, DateTime EnviadoEmUtc);

public sealed record ListarDocumentosAnexosQuery(Guid PessoaId);

public sealed record EnviarDocumentoAnexoCommand(Guid PessoaId, TipoDocumento Tipo, string NomeArquivo, string ContentType, byte[] Conteudo);

public sealed record ObterDocumentoAnexoQuery(Guid Id);

public sealed record RemoverDocumentoAnexoCommand(Guid Id);

public sealed class ListarDocumentosAnexosHandler(IAcademicoRepository repository)
    : IRequestHandler<ListarDocumentosAnexosQuery, IReadOnlyList<DocumentoAnexoDto>>
{
    public Task<IReadOnlyList<DocumentoAnexoDto>> HandleAsync(ListarDocumentosAnexosQuery request, CancellationToken cancellationToken = default) =>
        repository.ListarDocumentosAnexosAsync(request.PessoaId, cancellationToken);
}

public sealed class EnviarDocumentoAnexoHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<EnviarDocumentoAnexoCommand, DocumentoAnexoDto>
{
    public async Task<DocumentoAnexoDto> HandleAsync(EnviarDocumentoAnexoCommand request, CancellationToken cancellationToken = default)
    {
        if (await repository.GetAlunoByIdAsync(request.PessoaId, cancellationToken) is null
            && await repository.GetProfessorByIdAsync(request.PessoaId, cancellationToken) is null)
        {
            throw new UseCaseException("Aluno ou professor não encontrado.");
        }

        var documento = new DocumentoAnexo(request.PessoaId, request.Tipo, request.NomeArquivo, request.ContentType, request.Conteudo);
        await repository.AddDocumentoAnexoAsync(documento, cancellationToken);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Documento.Enviar", "DocumentoAnexo", documento.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new DocumentoAnexoDto(documento.Id, documento.Tipo, documento.NomeArquivo, documento.ContentType, documento.TamanhoBytes, documento.CreatedAtUtc);
    }
}

public sealed class ObterDocumentoAnexoHandler(IAcademicoRepository repository)
    : IRequestHandler<ObterDocumentoAnexoQuery, DocumentoAnexo?>
{
    public Task<DocumentoAnexo?> HandleAsync(ObterDocumentoAnexoQuery request, CancellationToken cancellationToken = default) =>
        repository.GetDocumentoAnexoByIdAsync(request.Id, cancellationToken);
}

public sealed class RemoverDocumentoAnexoHandler(IAcademicoRepository repository, ICurrentUserAccessor currentUser)
    : IRequestHandler<RemoverDocumentoAnexoCommand>
{
    public async Task HandleAsync(RemoverDocumentoAnexoCommand request, CancellationToken cancellationToken = default)
    {
        var documento = await repository.GetDocumentoAnexoByIdAsync(request.Id, cancellationToken)
            ?? throw new UseCaseException("Documento não encontrado.");

        repository.RemoveDocumentoAnexo(documento);
        await repository.AddLogAsync(
            new LogSistema(currentUser.AccountId, "Documento.Remover", "DocumentoAnexo", documento.Id, sucesso: true),
            cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
