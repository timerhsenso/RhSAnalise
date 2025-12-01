// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Tcbo1
// Data: 2025-12-01 01:28:26
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Tcbo1s;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Tcbo1s;

/// <summary>
/// Interface do serviço de API para Tabela de Ocupação.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITcbo1ApiService 
    : IApiService<Tcbo1Dto, CreateTcbo1Request, UpdateTcbo1Request, string>,
      IBatchDeleteService<string>
{
}
