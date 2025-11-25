// src/Web/Services/Sistemas/ISistemaApiService.cs
using RhSensoERP.Web.Models.Common;
using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Interface do serviço para comunicação com a API de Sistemas.
/// Implementa IBatchDeleteService para suportar exclusão em lote com resultado detalhado.
/// </summary>
public interface ISistemaApiService : 
    IApiService<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>,
    IBatchDeleteService<string>
{
    // A interface IBatchDeleteService já define o método DeleteBatchAsync
    // Não é necessário redeclarar aqui, pois a herança já o inclui
}
