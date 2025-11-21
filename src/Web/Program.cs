using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Bancos;
using RhSensoERP.Web.Filters;
using Serilog;

namespace RhSensoERP.Web;

/// <summary>
/// Classe principal da aplicação Web.
/// </summary>
public static class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// </summary>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ========================================
        // CONFIGURAÇÃO DE LOGGING COM SERILOG
        // ========================================
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/rhsensoerp-web-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog();

        // ========================================
        // CONFIGURAÇÃO DE SERVIÇOS
        // ========================================

        // Controllers e Views com filtros globais
        builder.Services.AddControllersWithViews(options =>
        {
            // Adiciona filtro global de exceções
            options.Filters.Add<GlobalExceptionFilter>();
            
            // Adiciona filtro global de validação de ModelState
            options.Filters.Add<ValidateModelStateFilter>();
        });

        // HttpContextAccessor (necessário para BaseApiService e TagHelpers)
        builder.Services.AddHttpContextAccessor();

        // HttpClient para comunicação com a API
        var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException("ApiSettings:BaseUrl não configurado.");

        builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(
                builder.Configuration.GetValue<int>("ApiSettings:Timeout", 30));
        });

        // Registra o HttpClient genérico para os serviços de API
        builder.Services.AddHttpClient("ApiClient", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.Timeout = TimeSpan.FromSeconds(
                builder.Configuration.GetValue<int>("ApiSettings:Timeout", 30));
        });

        // Registra os serviços de API
        builder.Services.AddScoped<IBancoApiService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("ApiClient");
            var logger = sp.GetRequiredService<ILogger<BancoApiService>>();
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            return new BancoApiService(httpClient, logger, httpContextAccessor);
        });

        // Autenticação com Cookies
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = builder.Configuration["Authentication:LoginPath"] ?? "/Account/Login";
                options.LogoutPath = builder.Configuration["Authentication:LogoutPath"] ?? "/Account/Logout";
                options.AccessDeniedPath = builder.Configuration["Authentication:AccessDeniedPath"] ?? "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(
                    builder.Configuration.GetValue<int>("Authentication:ExpireTimeSpan", 480));
                options.SlidingExpiration = builder.Configuration.GetValue<bool>("Authentication:SlidingExpiration", true);
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Name = builder.Configuration["Authentication:CookieName"] ?? "RhSensoERP.Auth";
            });

        builder.Services.AddAuthorization();

        // Session (opcional, se necessário)
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // ========================================
        // BUILD DA APLICAÇÃO
        // ========================================
        var app = builder.Build();

        // ========================================
        // PIPELINE DE REQUISIÇÕES
        // ========================================

        // Tratamento de erros
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Session (se configurado)
        app.UseSession();

        // Autenticação e Autorização
        app.UseAuthentication();
        app.UseAuthorization();

        // Rotas
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // ========================================
        // EXECUÇÃO
        // ========================================
        try
        {
            Log.Information("Iniciando RhSensoERP.Web...");
            await app.RunAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Erro fatal ao iniciar a aplicação");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}
