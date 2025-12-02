// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: SGC_TipoTreinamento
// Data: 2025-12-02 19:47:20
// AUTO-REGISTRO: Compatível com AddCrudToolServicesAutomatically()
// =============================================================================
using RhSensoERP.Web.Models.SGC_TipoTreinamentos;
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.SGC_TipoTreinamentos;

/// <summary>
/// Interface do serviço de API para Tipo Treinamento.
/// Herda de IApiService e IBatchDeleteService existentes.
/// </summary>
public interface ISGC_TipoTreinamentoApiService 
    : IApiService<SGC_TipoTreinamentoDto, CreateSGC_TipoTreinamentoRequest, UpdateSGC_TipoTreinamentoRequest, int>,
      IBatchDeleteService<int>
{
}
