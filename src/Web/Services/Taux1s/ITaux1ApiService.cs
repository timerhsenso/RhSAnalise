// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Taux1
// Data: 2025-12-01 23:06:17
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.Taux1s;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Taux1s;

/// <summary>
/// Interface do serviço de API para Tabela Auxiliar.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITaux1ApiService 
    : IApiService<Taux1Dto, CreateTaux1Request, UpdateTaux1Request, string>,
      IBatchDeleteService<string>
{
}
