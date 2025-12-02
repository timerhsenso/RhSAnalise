// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: SGC_TipoFornecedor
// Data: 2025-12-02 15:55:54
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.SGC_TipoFornecedores;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.SGC_TipoFornecedores;

/// <summary>
/// Interface do serviço de API para Tipo Fornecedor.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ISGC_TipoFornecedorApiService 
    : IApiService<SGC_TipoFornecedorDto, CreateSGC_TipoFornecedorRequest, UpdateSGC_TipoFornecedorRequest, int>,
      IBatchDeleteService<int>
{
}
