// src/Web/Controllers/BancosController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Bancos;
using RhSensoERP.Web.Services.Bancos;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Bancos.
/// </summary>
[Authorize]
public class BancosController : BaseCrudController<BancoDto, CreateBancoDto, UpdateBancoDto, string>
{
    public BancosController(
        IBancoApiService bancoApiService,
        ILogger<BancosController> logger)
        : base(bancoApiService, logger)
    {
    }

    /// <summary>
    /// Página principal (Index).
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new BancosListViewModel
        {
            // TODO: Buscar permissões reais do usuário logado
            UserPermissions = "IAEC" // I=Incluir, A=Alterar, E=Excluir, C=Consultar
        };

        return View(viewModel);
    }
}
