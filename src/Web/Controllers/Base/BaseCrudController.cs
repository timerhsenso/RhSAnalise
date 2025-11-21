// src/Web/Controllers/Base/BaseCrudController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Models.Base;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Controllers.Base;

/// <summary>
/// Controller abstrato para operações CRUD padrão.
/// Fornece métodos reutilizáveis para listagem, criação, edição e exclusão.
/// </summary>
/// <typeparam name="TDto">Tipo do DTO completo</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
/// <typeparam name="TKey">Tipo da chave primária</typeparam>
[Authorize]
public abstract class BaseCrudController<TDto, TCreateDto, TUpdateDto, TKey> : Controller
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IApiService<TDto, TCreateDto, TUpdateDto, TKey> _apiService;
    protected readonly ILogger _logger;

    protected BaseCrudController(
        IApiService<TDto, TCreateDto, TUpdateDto, TKey> apiService,
        ILogger logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    #region Mensagens TempData

    /// <summary>
    /// Define mensagem de sucesso.
    /// </summary>
    protected void SetSuccessMessage(string message)
    {
        TempData["SuccessMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de erro.
    /// </summary>
    protected void SetErrorMessage(string message)
    {
        TempData["ErrorMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de aviso.
    /// </summary>
    protected void SetWarningMessage(string message)
    {
        TempData["WarningMessage"] = message;
    }

    /// <summary>
    /// Define mensagem de informação.
    /// </summary>
    protected void SetInfoMessage(string message)
    {
        TempData["InfoMessage"] = message;
    }

    #endregion

    #region Respostas JSON Padronizadas

    /// <summary>
    /// Retorna resposta JSON de sucesso.
    /// </summary>
    protected IActionResult JsonSuccess(string message, object? data = null)
    {
        return Json(new
        {
            success = true,
            message,
            data
        });
    }

    /// <summary>
    /// Retorna resposta JSON de erro.
    /// </summary>
    protected IActionResult JsonError(string message, object? errors = null)
    {
        return Json(new
        {
            success = false,
            message,
            errors
        });
    }

    #endregion

    #region Actions CRUD Virtuais

    /// <summary>
    /// Action para listagem com DataTables (AJAX).
    /// </summary>
    [HttpPost]
    public virtual async Task<IActionResult> List([FromBody] DataTableRequest request)
    {
        try
        {
            // Calcula a página atual
            var page = (request.Start / request.Length) + 1;
            var pageSize = request.Length;
            var search = request.Search?.Value;

            // Busca os dados da API
            var result = await _apiService.GetPagedAsync(page, pageSize, search);

            if (!result.Success || result.Data == null)
            {
                return Json(new DataTableResponse<TDto>
                {
                    Draw = request.Draw,
                    RecordsTotal = 0,
                    RecordsFiltered = 0,
                    Data = new List<TDto>(),
                    Error = result.Message ?? "Erro ao buscar dados"
                });
            }

            // Retorna no formato esperado pelo DataTables
            return Json(new DataTableResponse<TDto>
            {
                Draw = request.Draw,
                RecordsTotal = result.Data.TotalCount,
                RecordsFiltered = result.Data.TotalCount,
                Data = result.Data.Items.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar listagem DataTables");
            return Json(new DataTableResponse<TDto>
            {
                Draw = request.Draw,
                RecordsTotal = 0,
                RecordsFiltered = 0,
                Data = new List<TDto>(),
                Error = "Erro ao buscar dados"
            });
        }
    }

    /// <summary>
    /// Action para obter registro por ID (AJAX).
    /// </summary>
    [HttpGet]
    public virtual async Task<IActionResult> GetById(TKey id)
    {
        try
        {
            var result = await _apiService.GetByIdAsync(id);

            if (!result.Success || result.Data == null)
            {
                return JsonError(result.Message ?? "Registro não encontrado");
            }

            return JsonSuccess("Registro encontrado", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar registro {Id}", id);
            return JsonError("Erro ao buscar registro");
        }
    }

    /// <summary>
    /// Action para criar registro (AJAX).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create([FromBody] TCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return JsonError("Dados inválidos", errors);
            }

            var result = await _apiService.CreateAsync(dto);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao criar registro", result.Errors);
            }

            return JsonSuccess(result.Message ?? "Registro criado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar registro");
            return JsonError("Erro ao criar registro");
        }
    }

    /// <summary>
    /// Action para atualizar registro (AJAX).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(TKey id, [FromBody] TUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(e => e.ErrorMessage).ToList()
                    );

                return JsonError("Dados inválidos", errors);
            }

            var result = await _apiService.UpdateAsync(id, dto);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao atualizar registro", result.Errors);
            }

            return JsonSuccess(result.Message ?? "Registro atualizado com sucesso", result.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar registro {Id}", id);
            return JsonError("Erro ao atualizar registro");
        }
    }

    /// <summary>
    /// Action para excluir registro (AJAX).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Delete(TKey id)
    {
        try
        {
            var result = await _apiService.DeleteAsync(id);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao excluir registro");
            }

            return JsonSuccess(result.Message ?? "Registro excluído com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir registro {Id}", id);
            return JsonError("Erro ao excluir registro");
        }
    }

    /// <summary>
    /// Action para excluir múltiplos registros (AJAX).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> DeleteMultiple([FromBody] IEnumerable<TKey> ids)
    {
        try
        {
            if (ids == null || !ids.Any())
            {
                return JsonError("Nenhum registro selecionado");
            }

            var result = await _apiService.DeleteMultipleAsync(ids);

            if (!result.Success)
            {
                return JsonError(result.Message ?? "Erro ao excluir registros");
            }

            return JsonSuccess(result.Message ?? "Registros excluídos com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir múltiplos registros");
            return JsonError("Erro ao excluir registros");
        }
    }

    #endregion

    #region Métodos Auxiliares

    /// <summary>
    /// Verifica se o usuário tem permissão para a ação.
    /// </summary>
    protected bool HasPermission(string cdFuncao, char acao)
    {
        // Busca a claim de permissão do usuário
        var permissionClaim = User.FindFirst($"permission:{cdFuncao}");
        if (permissionClaim != null)
        {
            return permissionClaim.Value.Contains(acao);
        }
        return false;
    }

    /// <summary>
    /// Obtém as permissões do usuário para uma função.
    /// </summary>
    protected string? GetUserPermissions(string cdFuncao)
    {
        var permissionClaim = User.FindFirst($"permission:{cdFuncao}");
        return permissionClaim?.Value;
    }

    #endregion
}
