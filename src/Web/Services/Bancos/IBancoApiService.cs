// =============================================================================
// RHSENSOERP WEB - BANCO API SERVICE INTERFACE
// =============================================================================
// Arquivo: src/Web/Services/Bancos/IBancoApiService.cs
// Descrição: Interface do serviço para comunicação com a API de Bancos
// =============================================================================

using RhSensoERP.Web.Models.Bancos;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Bancos;

/// <summary>
/// Interface para o serviço de API de Bancos.
/// </summary>
public interface IBancoApiService : IApiService<BancoDto, CreateBancoDto, UpdateBancoDto, string>
{
}
