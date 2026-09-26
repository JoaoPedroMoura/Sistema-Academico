using FaeterjAcademico.Application.Documents;
using FaeterjAcademico.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Documents;

/// <summary>Arquivos digitalizados dos documentos de alunos e professores (CPF, RG, histórico...).</summary>
[ApiController]
[Route("api/documentos")]
[Authorize(Roles = "Admin,Secretaria")]
public class DocumentosController(
    ListarDocumentosAnexosHandler listarHandler,
    EnviarDocumentoAnexoHandler enviarHandler,
    ObterDocumentoAnexoHandler obterHandler,
    RemoverDocumentoAnexoHandler removerHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DocumentoAnexoDto>>> Listar([FromQuery] Guid pessoaId, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarDocumentosAnexosQuery(pessoaId), cancellationToken));

    [HttpPost]
    [RequestSizeLimit(DocumentoAnexo.TamanhoMaximoBytes + 64 * 1024)]
    public async Task<ActionResult<DocumentoAnexoDto>> Enviar(
        [FromForm] Guid pessoaId, [FromForm] TipoDocumento tipo, IFormFile arquivo, CancellationToken cancellationToken)
    {
        using var memoria = new MemoryStream();
        await arquivo.CopyToAsync(memoria, cancellationToken);
        return Ok(await enviarHandler.HandleAsync(
            new EnviarDocumentoAnexoCommand(pessoaId, tipo, arquivo.FileName, arquivo.ContentType, memoria.ToArray()), cancellationToken));
    }

    [HttpGet("{id:guid}/arquivo")]
    public async Task<IActionResult> Baixar(Guid id, CancellationToken cancellationToken)
    {
        var documento = await obterHandler.HandleAsync(new ObterDocumentoAnexoQuery(id), cancellationToken);
        return documento is null ? NotFound() : File(documento.Conteudo, documento.ContentType, documento.NomeArquivo);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken cancellationToken)
    {
        await removerHandler.HandleAsync(new RemoverDocumentoAnexoCommand(id), cancellationToken);
        return NoContent();
    }
}
