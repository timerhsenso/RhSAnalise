// src/Web/Services/Sistemas/ISistemaApiService.cs

using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Interface do serviço para comunicação com a API de Sistemas.
/// </summary>
public interface ISistemaApiService : IApiService<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>
{
    // Métodos adicionais específicos podem ser adicionados aqui
}
