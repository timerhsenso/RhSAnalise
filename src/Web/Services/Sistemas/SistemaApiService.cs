// src/Web/Services/Sistemas/SistemaApiService.cs

using RhSensoERP.Web.Models.Sistemas;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Sistemas;

/// <summary>
/// Serviço para comunicação com a API de Sistemas.
/// </summary>
public class SistemaApiService : BaseApiService<SistemaDto, CreateSistemaDto, UpdateSistemaDto, string>, ISistemaApiService
{
    /// <summary>
    /// Endpoint base da API de Sistemas.
    /// </summary>
    private const string BaseEndpoint = "/api/identity/sistemas";

    public SistemaApiService(
        HttpClient httpClient,
        ILogger<SistemaApiService> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, logger, httpContextAccessor, BaseEndpoint)
    {
    }

    // Métodos adicionais específicos podem ser implementados aqui
}
