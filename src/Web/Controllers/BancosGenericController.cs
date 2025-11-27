// =============================================================================
// RHSENSOERP.WEB - BANCOS GENERIC CONTROLLER
// =============================================================================
// Arquivo: src/RhSensoERP.Web/Controllers/BancosGenericController.cs
// Descrição: Controller para Bancos usando a infraestrutura genérica de metadados
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Controllers.Base;
using RhSensoERP.Web.Services;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para gerenciamento de Bancos usando metadados.
/// Rota: /bancos-v2 (para não conflitar com o controller existente).
/// </summary>
[Route("bancos-v2")]
[Authorize]
public class BancosGenericController : GenericCrudController
{
    /// <summary>
    /// Nome da entidade.
    /// </summary>
    protected override string EntityName => "Banco";

    /// <summary>
    /// Nome do módulo.
    /// </summary>
    protected override string? ModuleName => "GestaoDePessoas";

    /// <summary>
    /// Exibir filtros avançados.
    /// </summary>
    protected override bool ShowAdvancedFilters => true;

    /// <summary>
    /// Construtor.
    /// </summary>
    public BancosGenericController(
        IMetadataService metadataService,
        ILogger<BancosGenericController> logger)
        : base(metadataService, logger)
    {
    }

    /// <summary>
    /// Rota padrão: GET /bancos-v2
    /// </summary>
    [HttpGet("")]
    public override Task<IActionResult> Index()
    {
        return base.Index();
    }
}