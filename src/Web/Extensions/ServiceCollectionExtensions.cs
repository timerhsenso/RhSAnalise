// =============================================================================
// RHSENSOERP WEB - SERVICE COLLECTION EXTENSIONS
// =============================================================================
// Arquivo: src/Web/Extensions/ServiceCollectionExtensions.cs
// Descrição: Métodos de extensão para registro de serviços no DI Container
// Versão: 3.2 (Adicionado suporte a serviços gerados pelo CrudTool)
// =============================================================================

using System.Net.Http.Headers;
using RhSensoERP.Web.Configuration;
using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Bancos;
using RhSensoERP.Web.Services.Sistemas;
//using RhSensoERP.Web.Services.Sitc2s;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Métodos de extensão para configuração de serviços da aplicação Web.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os serviços de API (HttpClients e implementações).
    /// </summary>
    /// <param name="services">Coleção de serviços do DI Container.</param>
    /// <param name="configuration">Configuração da aplicação.</param>
    /// <returns>A coleção de serviços para encadeamento.</returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =====================================================================
        // CONFIGURAÇÃO DE API SETTINGS
        // =====================================================================
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));

        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>();

        // Se não encontrar a seção, usa valores padrão
        if (apiSettings == null)
        {
            apiSettings = new ApiSettings
            {
                BaseUrl = "https://localhost:7193",
                TimeoutSeconds = 30,
                AuthTimeoutSeconds = 60,
                RetryCount = 3
            };
            Console.WriteLine("⚠️ [CONFIG] Seção 'ApiSettings' não encontrada. Usando valores padrão.");
        }

        // Valida se a BaseUrl está configurada
        if (string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
        {
            apiSettings.BaseUrl = "https://localhost:7193";
            Console.WriteLine("⚠️ [CONFIG] ApiSettings:BaseUrl vazio. Usando padrão: https://localhost:7193");
        }

        // =====================================================================
        // REGISTRO DO HTTPCONTEXTACCESSOR
        // =====================================================================
        services.AddHttpContextAccessor();

        // =====================================================================
        // HTTPCLIENT: ApiClient (Cliente Genérico)
        // =====================================================================
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        });

        // =====================================================================
        // HTTPCLIENT: AuthApiClient (Cliente de Autenticação)
        // =====================================================================
        services.AddHttpClient("AuthApiClient", client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSettings.AuthTimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        });

        // =====================================================================
        // REGISTRO DOS SERVIÇOS DE API (Core)
        // =====================================================================

        // Serviço de Autenticação
        services.AddScoped<IAuthApiService, AuthApiService>();

        // Serviço de Sistemas (usa IHttpClientFactory internamente via "ApiClient")
        services.AddScoped<ISistemaApiService, SistemaApiService>();

        // Serviço de Bancos
        services.AddScoped<IBancoApiService, BancoApiService>();

        // =====================================================================
        // SERVIÇO DE METADADOS (UI Dinâmica)
        // =====================================================================
        services.AddHttpClient<IMetadataService, MetadataService>(client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        });

        // =====================================================================
        // SERVIÇOS GERADOS PELO CRUDTOOL
        // =====================================================================
        // Os serviços abaixo foram gerados automaticamente pelo RhSensoERP.CrudTool
        // Para adicionar novos serviços, siga o padrão:
        // services.AddHttpClient<I{Entity}ApiService, {Entity}ApiService>(ConfigureHttpClient(apiSettings));

        // Serviço de Situação de Frequência (Sitc2) - Módulo ControleDePonto
       /* services.AddHttpClient<ISitc2ApiService, Sitc2ApiService>(client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        });
       */
        // =====================================================================
        // LOG DE CONFIGURAÇÃO
        // =====================================================================
        Console.WriteLine($"✅ [CONFIG] API Services configurados | BaseUrl: {apiSettings.BaseUrl} | Timeout: {apiSettings.TimeoutSeconds}s");

        return services;
    }
}