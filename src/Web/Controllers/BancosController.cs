// =============================================================================
// RHSENSOERP WEB - BANCOS CONTROLLER (ATUALIZADO COM CACHE DE PERMISSÕES)
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Bancos;
using RhSensoERP.Web.Services.Bancos;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Bancos.
/// Herda toda a funcionalidade CRUD do BaseCrudController com verificação de permissões.
/// </summary>
[Authorize]
public class BancosController : BaseCrudController<BancoDto, CreateBancoDto, UpdateBancoDto, string>
{
    private const string CdFuncao = "SEG_BANCOS"; // Define a função para este controller

    public BancosController(
        IBancoApiService bancoApiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<BancosController> logger)
        : base(bancoApiService, permissionsCache, logger)
    {
    }

    /// <summary>
    /// Página principal (Index) com verificação de permissão de consulta.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Verifica a permissão de consulta ANTES de renderizar a página
        if (!await CanViewAsync(CdFuncao, ct))
        {
            return RedirectToAction("AccessDenied", "Account");
        }

        var viewModel = new BancosListViewModel
        {
            // Busca as permissões reais do cache e as envia para a View
            UserPermissions = await GetUserPermissionsAsync(CdFuncao, ct)
        };

        return View(viewModel);
    }

    /// <summary>
    /// Override do método Create para adicionar verificação de permissão.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] CreateBancoDto dto)
    {
        if (!await CanCreateAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para criar registros.");
        }
        return await base.Create(dto);
    }

    /// <summary>
    /// Override do método Update para adicionar verificação de permissão.
    /// </summary>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update(string id, [FromBody] UpdateBancoDto dto)
    {
        if (!await CanEditAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para alterar registros.");
        }
        return await base.Update(id, dto);
    }

    /// <summary>
    /// Override do método Delete para adicionar verificação de permissão.
    /// </summary>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(string id)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para excluir registros.");
        }
        return await base.Delete(id);
    }

    /// <summary>
    /// Override do método DeleteMultiple para adicionar verificação de permissão.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<string> ids)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para excluir registros.");
        }
        return await base.DeleteMultiple(ids);
    }
}
