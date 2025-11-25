// =============================================================================
// RHSENSOERP WEB - SERVICE COLLECTION EXTENSIONS
// =============================================================================
// Arquivo: src/Web/Extensions/ServiceCollectionExtensions.cs
// Descrição: Métodos de extensão para registro de serviços no DI Container
// Versão: 2.1 (Corrigido - BaseAddress configurado)
// Data: 25/11/2025
//
// CORREÇÕES APLICADAS:
// - BaseAddress do HttpClient agora é configurado a partir do appsettings.json
// - Handler de timeout configurado
// - Logging de requisições HTTP habilitado
// =============================================================================

using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using RhSensoERP.Web.Configuration;
using RhSensoERP.Web.Services;

namespace RhSensoERP.Web.Extensions;

/// <summary>
/// Métodos de extensão para configuração de serviços da aplicação Web.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra todos os serviços de API (HttpClients e implementações).
    /// Configura HttpClient com BaseAddress, timeout, retry policies e logging.
    /// </summary>
    /// <param name="services">Container de serviços</param>
    /// <param name="configuration">Configuração da aplicação</param>
    /// <returns>IServiceCollection para encadeamento</returns>
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // =====================================================================
        // CONFIGURAÇÃO DE API SETTINGS
        // =====================================================================
        // Lê as configurações da seção "ApiSettings" do appsettings.json
        services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
        
        // Obtém as configurações para uso imediato
        var apiSettings = configuration.GetSection("ApiSettings").Get<ApiSettings>()
            ?? throw new InvalidOperationException(
                "Seção 'ApiSettings' não encontrada no appsettings.json. " +
                "Verifique se a configuração está correta.");

        // Valida se a BaseUrl está configurada
        if (string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
        {
            throw new InvalidOperationException(
                "ApiSettings:BaseUrl não está configurado. " +
                "Adicione a URL da API no appsettings.json.");
        }

        // =====================================================================
        // REGISTRO DO HTTPCONTEXTACCESSOR
        // =====================================================================
        // Necessário para acessar HttpContext em serviços (ex: obter token JWT)
        services.AddHttpContextAccessor();

        // =====================================================================
        // POLÍTICA DE RETRY COM POLLY
        // =====================================================================
        // Configura retry automático para falhas transitórias (timeout, 5xx, etc)
        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(
                retryCount: apiSettings.RetryCount,
                sleepDurationProvider: retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    // Log do retry (será capturado pelo ILogger do HttpClient)
                });

        // Política de circuit breaker
        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30));

        // =====================================================================
        // HTTPCLIENT: ApiClient (Cliente Genérico)
        // =====================================================================
        // Cliente base usado por todos os serviços de API
        services.AddHttpClient("ApiClient", (sp, client) =>
        {
            // 🔧 CORREÇÃO: Define o BaseAddress a partir da configuração
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            
            // Timeout padrão
            client.Timeout = TimeSpan.FromSeconds(apiSettings.TimeoutSeconds);
            
            // Headers padrão
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            // User-Agent para identificação
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        })
        .AddPolicyHandler(retryPolicy)
        .AddPolicyHandler(circuitBreakerPolicy);

        // =====================================================================
        // HTTPCLIENT: AuthApiClient (Cliente de Autenticação)
        // =====================================================================
        // Cliente específico para endpoints de autenticação
        // Não usa retry em login para evitar bloqueios por rate limit
        services.AddHttpClient("AuthApiClient", (sp, client) =>
        {
            // 🔧 CORREÇÃO: Define o BaseAddress a partir da configuração
            client.BaseAddress = new Uri(apiSettings.BaseUrl);
            
            // Timeout maior para operações de autenticação
            client.Timeout = TimeSpan.FromSeconds(apiSettings.AuthTimeoutSeconds);
            
            // Headers padrão
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            
            // User-Agent para identificação
            client.DefaultRequestHeaders.UserAgent.ParseAdd("RhSensoERP.Web/2.0");
        });
        // Nota: Não adiciona retry policy em auth para evitar múltiplas tentativas de login

        // =====================================================================
        // REGISTRO DOS SERVIÇOS DE API
        // =====================================================================

        // Serviço de Autenticação
        services.AddScoped<IAuthApiService, AuthApiService>();

        // Serviço de Sistemas
        services.AddScoped<ISistemaApiService, SistemaApiService>();

        // Serviço de Bancos (se existir)
        // services.AddScoped<IBancoApiService, BancoApiService>();

        // =====================================================================
        // LOG DE CONFIGURAÇÃO
        // =====================================================================
        var logger = services.BuildServiceProvider()
            .GetService<ILoggerFactory>()?
            .CreateLogger("ServiceCollectionExtensions");
        
        logger?.LogInformation(
            "✅ API Services configurados | BaseUrl: {BaseUrl} | Timeout: {Timeout}s | Retry: {Retry}x",
            apiSettings.BaseUrl,
            apiSettings.TimeoutSeconds,
            apiSettings.RetryCount);

        return services;
    }
}
