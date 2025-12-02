// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoFornecedor
// Data: 2025-12-02 15:55:54
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.SGC_TipoFornecedores;
using RhSensoERP.Web.Services.SGC_TipoFornecedores;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Controllers;

// =============================================================================
// MENU ITEM - Configuração para aparecer no menu dinâmico
// =============================================================================
[MenuItem(
    Module = MenuModule.GestaoDePessoas,
    DisplayName = "Tipo Fornecedor",
    Icon = "fas fa-table",
    Order = 10,
    CdFuncao = "RHU_FM_TAUX1"
)]

/// <summary>
/// Controller para gerenciamento de Tipo Fornecedor.
/// Herda toda a funcionalidade CRUD de BaseCrudController.
/// </summary>
[Authorize]
public class SGC_TipoFornecedoresController 
    : BaseCrudController<SGC_TipoFornecedorDto, CreateSGC_TipoFornecedorRequest, UpdateSGC_TipoFornecedorRequest, int>
{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código da função/tela no sistema de permissões.
    /// Corresponde ao cadastrado na tabela tfunc1 do banco legado.
    /// </summary>
    private const string CdFuncao = "RHU_FM_TAUX1";

    /// <summary>
    /// Código do sistema ao qual esta função pertence.
    /// </summary>
    private const string CdSistema = "RHU";

    private readonly ISGC_TipoFornecedorApiService _sgc_tipofornecedorService;

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public SGC_TipoFornecedoresController(
        ISGC_TipoFornecedorApiService apiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<SGC_TipoFornecedoresController> logger)
        : base(apiService, permissionsCache, logger)
    {
        _sgc_tipofornecedorService = apiService;
    }

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    /// <summary>
    /// Página principal com verificação de permissão de consulta.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        // Verifica permissão de consulta
        if (!await CanViewAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "Acesso negado: Usuário {User} tentou acessar {Funcao} sem permissão",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction("AccessDenied", "Account");
        }

        // Busca permissões do usuário para esta função
        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new SGC_TipoFornecedoresListViewModel
        {
            UserPermissions = permissions
        };

        _logger.LogDebug(
            "Usuário {User} acessou {Funcao} | Permissões: {Permissions}",
            User.Identity?.Name,
            CdFuncao,
            permissions);

        return View(viewModel);
    }

    // =========================================================================
    // ACTION: GET BY ID (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Busca registro por ID via AJAX.
    /// </summary>
    [HttpGet]
    public override async Task<IActionResult> GetById(int id)
    {
        if (!await CanViewAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para visualizar registros.");
        }

        return await base.GetById(id);
    }

    // =========================================================================
    // ACTION: CREATE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Cria um novo registro.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] CreateSGC_TipoFornecedorRequest dto)
    {
        if (!await CanCreateAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de inclusão negada: Usuário {User} sem permissão 'I' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para criar registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} criando registro em {Funcao}",
            User.Identity?.Name,
            CdFuncao);

        return await base.Create(dto);
    }

    // =========================================================================
    // ACTION: EDIT (POST para compatibilidade com CrudBase.js)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via POST.
    /// CrudBase.js envia para /Edit?id=xxx via POST.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] int id, [FromBody] UpdateSGC_TipoFornecedorRequest dto)
    {
        if (EqualityComparer<int>.Default.Equals(id, default))
        {
            return JsonError("ID do registro não informado.");
        }

        if (!await CanEditAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de alteração negada: Usuário {User} sem permissão 'A' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} alterando registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: UPDATE (PUT padrão REST)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via PUT.
    /// </summary>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update(int id, [FromBody] UpdateSGC_TipoFornecedorRequest dto)
    {
        if (!await CanEditAsync(CdFuncao))
        {
            return JsonError("Você não tem permissão para alterar registros nesta tela.");
        }

        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: DELETE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Exclui um registro.
    /// CrudBase.js envia DELETE para /Delete?id=xxx
    /// </summary>
    [HttpPost]
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(int id)
    {
        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de exclusão negada: Usuário {User} sem permissão 'E' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} excluindo registro {Id} em {Funcao}",
            User.Identity?.Name,
            id,
            CdFuncao);

        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE MULTIPLE (Sobrescrito para verificar permissão)
    // =========================================================================

    /// <summary>
    /// Exclui múltiplos registros.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
    {
        if (ids == null || ids.Count == 0)
        {
            return JsonError("Nenhum registro selecionado para exclusão.");
        }

        if (!await CanDeleteAsync(CdFuncao))
        {
            _logger.LogWarning(
                "Tentativa de exclusão múltipla negada: Usuário {User} sem permissão 'E' em {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para excluir registros nesta tela.");
        }

        _logger.LogInformation(
            "Usuário {User} excluindo {Count} registros em {Funcao}",
            User.Identity?.Name,
            ids.Count,
            CdFuncao);

        return await base.DeleteMultiple(ids);
    }
}
