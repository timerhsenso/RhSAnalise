// src/Web/Controllers/SistemasController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Sistemas.
/// </summary>
[Authorize]
public class SistemasController : BaseCrudController<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>
{
    public SistemasController(
        ISistemaApiService sistemaApiService,
        ILogger<SistemasController> logger)
        : base(sistemaApiService, logger)
    {
    }

    /// <summary>
    /// Página principal (Index).
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        var viewModel = new SistemasListViewModel
        {
            // TODO: Buscar permissões reais do usuário logado
            UserPermissions = "IAEC" // I=Incluir, A=Alterar, E=Excluir, C=Consultar
        };

        return View(viewModel);
    }
}
