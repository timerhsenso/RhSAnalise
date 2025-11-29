// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.5
// Entity: Sitc2
// Data: 2025-11-28 21:48:47
// =============================================================================
using RhSensoERP.Web.Models.Sitc2s;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sitc2s;

/// <summary>
/// Interface do serviço de API para Situação de Frequência.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ISitc2ApiService 
    : IApiService<Sitc2Dto, CreateSitc2Request, UpdateSitc2Request, Guid>,
      IBatchDeleteService<Guid>
{
}
