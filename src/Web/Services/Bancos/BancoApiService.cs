// src/Web/Services/Bancos/BancoApiService.cs

using RhSensoERP.Web.Models.Bancos;
using RhSensoERP.Web.Services.Base;

namespace RhSensoERP.Web.Services.Bancos;

/// <summary>
/// Interface para o serviço de API de Bancos.
/// </summary>
public interface IBancoApiService : IApiService<BancoDto, CreateBancoDto, UpdateBancoDto, string>
{
}

/// <summary>
/// Serviço de comunicação com a API de Bancos.
/// </summary>
public sealed class BancoApiService : BaseApiService<BancoDto, CreateBancoDto, UpdateBancoDto, string>, IBancoApiService
{
    public BancoApiService(
        HttpClient httpClient,
        ILogger<BancoApiService> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(httpClient, logger, httpContextAccessor, "api/v1/gestaodepessoas/tabelas/pessoal/bancos")
    {
    }
}
