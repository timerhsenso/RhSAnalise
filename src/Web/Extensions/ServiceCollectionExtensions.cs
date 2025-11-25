using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Bancos;
using RhSensoERP.Web.Services.Sistemas;

namespace RhSensoERP.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl não configurado.");

        var timeout = TimeSpan.FromSeconds(configuration.GetValue<int>("ApiSettings:Timeout", 30));

        services.AddHttpContextAccessor();

        // Registra o HttpClient genérico para os serviços de API
        services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = timeout;
        });

        // Serviço de Autenticação
        services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = timeout;
        });

        // Registra os serviços de API que dependem do HttpClient genérico
        services.AddScoped<ISistemaApiService, SistemaApiService>();
        services.AddScoped<IBancoApiService, BancoApiService>();
        // Adicione outros serviços de API aqui

        return services;
    }
}