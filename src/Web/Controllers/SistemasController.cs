// =============================================================================
// RHSENSOERP WEB - SISTEMAS CONTROLLER (COM CONTROLE DE BOTÕES)
// =============================================================================
// Arquivo: src/Web/Controllers/SistemasController.cs
// Versão: 3.1 - Corrigido para suportar POST /Edit (compatibilidade com CrudBase.js)
// =============================================================================
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Permissions;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Sistemas.
/// Herda toda a funcionalidade CRUD do BaseCrudController com verificação de permissões.
/// </summary>
[Authorize]
public class SistemasController : BaseCrudController<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>
{
    // =========================================================================
    // CONFIGURAÇÃO DE PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Código da função/tela no sistema de permissões.
    /// Este código deve corresponder ao cadastrado na tabela tfunc1 do banco legado.
    /// </summary>
    private const string CdFuncao = "SEG_FM_TSISTEMA";

    /// <summary>
    /// Código do sistema ao qual esta função pertence.
    /// Sistemas pertence ao módulo SEG (Segurança).
    /// </summary>
    private const string CdSistema = "SEG";

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public SistemasController(
        ISistemaApiService sistemaApiService,
        IUserPermissionsCacheService permissionsCache,
        ILogger<SistemasController> logger)
        : base(sistemaApiService, permissionsCache, logger)
    {
    }

    // =========================================================================
    // ACTION: INDEX (Página Principal)
    // =========================================================================

    /// <summary>
    /// Página principal (Index) com verificação de permissão de consulta.
    /// Valida se o usuário tem permissão de CONSULTAR (C) esta função.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Verifica a permissão de consulta ANTES de renderizar a página
        if (!await CanViewAsync(CdFuncao, ct))
        {
            _logger.LogWarning(
                "⛔ Acesso negado: Usuário {User} tentou acessar {Funcao} sem permissão de consulta",
                User.Identity?.Name,
                CdFuncao);

            return RedirectToAction("AccessDenied", "Account");
        }

        // Busca as permissões específicas do usuário para esta função
        var permissions = await GetUserPermissionsAsync(CdFuncao, ct);

        var viewModel = new SistemasListViewModel
        {
            // ⭐ BaseListViewModel já possui a propriedade UserPermissions
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

    /// <summary>
    /// Cria um novo registro.
    /// Valida se o usuário tem permissão de INCLUIR (I) nesta função.
    /// </summary>
    /// <param name="dto">Dados do registro a ser criado</param>
    /// <returns>JSON com resultado da operação</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([FromBody] CreateSistemaDto dto)
    {
        // Verifica se o usuário tem permissão de inclusão
        if (!await CanCreateAsync(CdFuncao))
        {
            _logger.LogWarning(
                "⛔ Tentativa de inclusão negada: Usuário {User} não tem permissão 'I' na função {Funcao}",
                User.Identity?.Name,
                CdFuncao);

            return JsonError("Você não tem permissão para criar registros nesta tela.");
        }

        _logger.LogInformation(
            "➕ Usuário {User} está criando um novo registro em {Funcao}",
            User.Identity?.Name,
            CdFuncao);

        // Chama o método base que já implementa toda a lógica de criação
        return await base.Create(dto);
    }

    // =========================================================================
    // ACTION: EDIT (Alterar via POST - compatibilidade com CrudBase.js)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via POST.
    /// Esta action é necessária para compatibilidade com o CrudBase.js que faz POST para /Edit.
    /// Valida se o usuário tem permissão de ALTERAR (A) nesta função.
    /// </summary>
    /// <param name="id">ID do registro a ser atualizado (via query string)</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>JSON com resultado da operação</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromQuery] string id, [FromBody] UpdateSistemaDto dto)
    {
        // Validação do ID
        if (string.IsNullOrWhiteSpace(id))
        {
            _logger.LogWarning("⛔ Tentativa de edição sem ID informado");
            return JsonError("ID do registro não informado.");
        }

        // Verifica se o usuário tem permissão de alteração
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

        // Reutiliza a lógica do método Update do BaseCrudController
        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: UPDATE (Alterar via PUT - padrão REST)
    // =========================================================================

    /// <summary>
    /// Atualiza um registro existente via PUT (padrão REST).
    /// Valida se o usuário tem permissão de ALTERAR (A) nesta função.
    /// </summary>
    /// <param name="id">ID do registro a ser atualizado</param>
    /// <param name="dto">Dados atualizados</param>
    /// <returns>JSON com resultado da operação</returns>
    [HttpPut]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Update(string id, [FromBody] UpdateSistemaDto dto)
    {
        // Verifica se o usuário tem permissão de alteração
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

        // Chama o método base que já implementa toda a lógica de atualização
        return await base.Update(id, dto);
    }

    // =========================================================================
    // ACTION: DELETE (Excluir)
    // =========================================================================

    /// <summary>
    /// Exclui um registro.
    /// Valida se o usuário tem permissão de EXCLUIR (E) nesta função.
    /// </summary>
    /// <param name="id">ID do registro a ser excluído</param>
    /// <returns>JSON com resultado da operação</returns>
    [HttpDelete]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(string id)
    {
        // Verifica se o usuário tem permissão de exclusão
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

        // Chama o método base que já implementa toda a lógica de exclusão
        return await base.Delete(id);
    }

    // =========================================================================
    // ACTION: DELETE MULTIPLE (Excluir Múltiplos)
    // =========================================================================

    /// <summary>
    /// Exclui múltiplos registros de uma vez.
    /// Valida se o usuário tem permissão de EXCLUIR (E) nesta função.
    /// </summary>
    /// <param name="ids">Lista de IDs a serem excluídos</param>
    /// <returns>JSON com resultado da operação</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteMultiple([FromBody] List<string>? ids)
    {
        // Validação de parâmetro nulo
        if (ids == null || ids.Count == 0)
        {
            return JsonError("Nenhum registro selecionado para exclusão.");
        }

        // Verifica se o usuário tem permissão de exclusão
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

        // Chama o método base que já implementa toda a lógica de exclusão múltipla
        return await base.DeleteMultiple(ids);
    }
}