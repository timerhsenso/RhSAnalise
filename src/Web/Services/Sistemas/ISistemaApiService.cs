// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Interface do serviço de API para Sistema.
/// </summary>
public interface ISistemaApiService 
    : IApiService<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>,
      IBatchDeleteService<string>
{
}
