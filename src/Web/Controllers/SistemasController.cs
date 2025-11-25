// src/Web/Controllers/SistemasController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Sistemas.
/// Herda toda a funcionalidade CRUD do BaseCrudController, incluindo exclusão em lote.
/// </summary>
[Authorize]
public class SistemasController : BaseCrudController<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>
{
    private readonly ISistemaApiService _sistemaService;

    public SistemasController(
        ISistemaApiService sistemaApiService,
        ILogger<SistemasController> logger)
        : base(sistemaApiService, logger)
    {
        _sistemaService = sistemaApiService;
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
            // Exemplo: UserPermissions = GetUserPermissions("SISTEMAS")
            UserPermissions = "IAEC" // I=Incluir, A=Alterar, E=Excluir, C=Consultar
        };

        return View(viewModel);
    }

    // NOTA: O método DeleteMultiple foi REMOVIDO deste controller.
    // A funcionalidade de exclusão múltipla agora é fornecida automaticamente
    // pelo BaseCrudController, que detecta que ISistemaApiService implementa
    // IBatchDeleteService<string> e usa o método DeleteBatchAsync com resultado detalhado.
    //
    // Benefícios desta abordagem:
    // 1. Eliminação de código duplicado
    // 2. Reutilização automática em todos os controllers CRUD
    // 3. Manutenção centralizada no controller base
    // 4. Suporte automático a exclusão detalhada quando o serviço implementa IBatchDeleteService
    // 5. Fallback automático para exclusão simples quando o serviço não implementa a interface
}
