using FaeterjAcademico.Application.Common;
using FaeterjAcademico.Application.Materials.Dtos;
using FaeterjAcademico.Application.Materials.EnviarMaterial;
using FaeterjAcademico.Application.Materials.ListarMateriais;
using FaeterjAcademico.Application.Materials.ListarMeusMateriais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FaeterjAcademico.Api.Controllers.Materials;

/// <summary>Upload (Perfil Professor) e consulta/download (Perfil Aluno) de materiais complementares (ANALISE-TCC.md §2).</summary>
[ApiController]
[Route("api/materiais")]
public class MateriaisController(
    EnviarMaterialHandler enviarHandler,
    ListarMateriaisHandler listarHandler,
    ListarMeusMateriaisHandler listarMeusHandler,
    ICurrentUserAccessor currentUser) : ControllerBase
{
    private Guid AccountIdAutenticado =>
        currentUser.AccountId ?? throw new UseCaseException("Usuário autenticado não identificado.");

    [HttpGet("meus")]
    [Authorize(Roles = "Aluno")]
    public async Task<ActionResult<IReadOnlyList<MeuMaterialDto>>> ListarMeus(CancellationToken cancellationToken) =>
        Ok(await listarMeusHandler.HandleAsync(new ListarMeusMateriaisQuery(AccountIdAutenticado), cancellationToken));

    [HttpGet]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<IReadOnlyList<MaterialDto>>> Listar([FromQuery] Guid turmaId, CancellationToken cancellationToken) =>
        Ok(await listarHandler.HandleAsync(new ListarMateriaisQuery(turmaId), cancellationToken));

    [HttpPost]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<MaterialDto>> Enviar(EnviarMaterialRequest request, CancellationToken cancellationToken)
    {
        var material = await enviarHandler.HandleAsync(
            new EnviarMaterialCommand(
                AccountIdAutenticado, request.TurmaId, request.Titulo, request.Descricao,
                request.ArquivoUrl, request.ArquivoNomeOriginal, request.TamanhoBytes),
            cancellationToken);

        return CreatedAtAction(nameof(Listar), new { turmaId = request.TurmaId }, material);
    }
}
