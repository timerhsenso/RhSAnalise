// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Permissions;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Sistema.
/// Herda toda a funcionalidade CRUD do BaseCrudController com verificação de permissões.
/// </summary>
[Authorize]
public class SistemasController : BaseCrudController<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>
{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    private const string CdFuncao = "SEG_FM_TSISTEMA";
    private const string CdSistema = "SEG";

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public SistemasController(
        ISistemaApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<SistemasController> logger)
        : base(apiService, permissionsCache, logger)
    {
    }

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        if (!await CanViewAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "⛔ Acesso negado: Usuário {User} tentou acessar {Funcao} sem permissão de consulta",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction("AccessDenied", "Account");
        }

        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new SistemasListViewModel
        {
            UserPermissions = permissions
        };

        _logger.LogInformation(
            "✅ Usuário {User} acessou {Funcao} | Permissões: I={CanCreate}, A={CanEdit}, E={CanDelete}, C={CanView}",
            User.Identity?.Name,
            CdFuncao,
            viewModel.CanCreate,
            viewModel.CanEdit,
            viewModel.CanDelete,
            viewModel.CanView);

        return View(viewModel);
    }

    // =========================================================================
    // ACTION: CREATE (Incluir)
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] CreateSistemaDto dto)
    {
        if (!await CanCreateAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de inclusão negada: Usuário {User} não tem permissão 'I' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para criar registros nesta tela.");
        }

        // Log do DTO recebido para debug
        _logger.LogDebug(
            "➕ Create DTO recebido: CdSistema={CdSistema}, DcSistema={DcSistema}, Ativo={Ativo}",
            dto?.CdSistema,
            dto?.DcSistema,
            dto?.Ativo);

        if (dto == null)
        {
            return JsonError("Dados inválidos.");
        }

        if (string.IsNullOrWhiteSpace(dto.CdSistema))
        {
            return JsonError("O Código do Sistema é obrigatório.");
        }

        _logger.LogInformation(
            "➕ Usuário {User} está criando um novo registro em {Funcao}",
            User.Identity?.Name,
            CdFuncao);

        return await base.Create(dto);
    }

    // =========================================================================
    // ACTION: EDIT (Alterar via POST - compatibilidade com CrudBase.js)
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] string id, [FromBody] UpdateSistemaDto dto)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("⛔ Tentativa de edição sem ID informado");
            return JsonError("ID do registro não informado.");
        }

        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de alteração negada: Usuário {User} não tem permissão 'A' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "✏️ Usuário {User} está alterando registro {Id} em {Funcao} (via Edit POST)",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: UPDATE (Alterar via PUT - padrão REST)
    // =========================================================================

    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update(string id, [FromBody] UpdateSistemaDto dto)
    {
        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de alteração negada: Usuário {User} não tem permissão 'A' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "✏️ Usuário {User} está alterando registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: DELETE (Excluir via DELETE - padrão REST)
    // =========================================================================

    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(string id)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de exclusão negada: Usuário {User} não tem permissão 'E' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "🗑️ Usuário {User} está excluindo registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE VIA POST (Compatibilidade com JavaScript/AJAX)
    // =========================================================================

    /// <summary>
    /// Exclui um registro via POST (compatibilidade com CrudBase.js que usa POST).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("[controller]/Delete")]
    public async Task<IActionResult> DeletePost([FromQuery] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return JsonError("ID do registro não informado.");
        }

        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de exclusão negada: Usuário {User} não tem permissão 'E' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "🗑️ Usuário {User} está excluindo registro {Id} em {Funcao} (via POST)",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE MULTIPLE (Excluir Múltiplos)
    // =========================================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<string>? ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return JsonError("Nenhum registro selecionado para exclusão.");
        }

        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de exclusão múltipla negada: Usuário {User} não tem permissão 'E' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "🗑️ Usuário {User} está excluindo {Count} registros em {Funcao}",
            User.Identity?.Name,
            ids.Count,
            CdFuncao);

        return await base.DeleteMultiple(ids);
    }
}