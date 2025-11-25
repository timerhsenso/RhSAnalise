// =============================================================================
// RHSENSOERP WEB - SERVICE COLLECTION EXTENSIONS
// =============================================================================
// Arquivo: src/Web/Extensions/ServiceCollectionExtensions.cs
// Descrição: Métodos de extensão para registro de serviços no DI Container
// Versão: 3.0 (Corrigido - Sem Polly, todos os serviços registrados)
// =============================================================================

using System.Net.Http.Headers;
using RhSensoERP.Web.Configuration;
using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Bancos;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Métodos de extensão para configuração de serviços da aplicação Web.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os serviços de API (HttpClients e implementações).
    /// </summary>
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
        // REGISTRO DOS SERVIÇOS DE API
        // =====================================================================

        // Serviço de Autenticação
        services.AddScoped<IAuthApiService, AuthApiService>();

        // Serviço de Sistemas
        services.AddScoped<ISistemaApiService, SistemaApiService>();

        // Serviço de Bancos
        services.AddScoped<IBancoApiService, BancoApiService>();

        // =====================================================================
        // LOG DE CONFIGURAÇÃO
        // =====================================================================
        Console.WriteLine($"✅ [CONFIG] API Services configurados | BaseUrl: {apiSettings.BaseUrl} | Timeout: {apiSettings.TimeoutSeconds}s");

        return services;
    }
}
