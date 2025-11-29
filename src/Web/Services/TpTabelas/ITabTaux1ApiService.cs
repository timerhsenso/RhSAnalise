// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.5
// Entity: TabTaux1
// Data: 2025-11-28 23:45:52
// =============================================================================
using RhSensoERP.Web.Models.TpTabelas;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.TpTabelas;

/// <summary>
/// Interface do serviço de API para Tipo de Tabela.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ITabTaux1ApiService 
    : IApiService<TabTaux1Dto, CreateTabTaux1Request, UpdateTabTaux1Request, string>,
      IBatchDeleteService<string>
{
}
