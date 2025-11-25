// ============================================================================
// RHSENSOERP WEB - PROGRAM.CS
// ============================================================================
// Arquivo: Program.cs
// Descrição: Ponto de entrada da aplicação Web ASP.NET Core 8
// Versão: 2.0 (Refatorado)
// Data: 24/11/2025
// 
// Responsabilidades:
// - Configuração do WebApplicationBuilder (serviços, logging, autenticação)
// - Configuração do pipeline de requisições HTTP (middlewares)
// - Inicialização e execução da aplicação
// 
// Melhorias Aplicadas:
// - Eliminação de duplicação no registro de serviços de API
// - Centralização da configuração de HttpClients no método de extensão
// - Documentação XML completa para facilitar manutenção
// - Uso de ConfigureAwait(false) para melhor performance
// - Organização clara das seções de configuração
// ============================================================================

using Microsoft.AspNetCore.Authentication.Cookies;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Filters;
using Serilog;

namespace RhSensoERP.Web;

/// <summary>
/// Classe principal da aplicação Web RhSensoERP.
/// Configura os serviços, o pipeline de requisições e inicia a aplicação.
/// </summary>
public static class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// Configura o WebApplicationBuilder, registra serviços, define o pipeline HTTP
    /// e executa a aplicação de forma assíncrona.
    /// </summary>
    /// <param name="args">Argumentos de linha de comando passados para a aplicação.</param>
    /// <returns>Task representando a execução assíncrona da aplicação.</returns>
    public static async Task Main(string[] args)
    {
        // Cria o builder da aplicação Web
        var builder = WebApplication.CreateBuilder(args);

        // ========================================
        // CONFIGURAÇÃO DE LOGGING COM SERILOG
        // ========================================
        // Serilog é configurado para:
        // - Ler configurações do appsettings.json
        // - Enriquecer logs com contexto adicional
        // - Escrever logs no console (para desenvolvimento/Docker)
        // - Escrever logs em arquivos rotativos diários (para produção)
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                path: "logs/rhsensoerp-web-.log",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        // Substitui o logger padrão do ASP.NET Core pelo Serilog
        builder.Host.UseSerilog();

        // ========================================
        // CONFIGURAÇÃO DE SERVIÇOS (DI CONTAINER)
        // ========================================

        // Registra Controllers e Views com filtros globais
        // Filtros globais são aplicados a todas as actions de todos os controllers
        builder.Services.AddControllersWithViews(options =>
        {
            // GlobalExceptionFilter: Captura exceções não tratadas e retorna respostas padronizadas
            options.Filters.Add<GlobalExceptionFilter>();

            // ValidateModelStateFilter: Valida automaticamente o ModelState antes de executar actions
            options.Filters.Add<ValidateModelStateFilter>();
        });

        // Registra todos os serviços de API (HttpClients e implementações)
        // Este método de extensão centraliza a configuração de:
        // - HttpClient genérico "ApiClient"
        // - IAuthApiService (autenticação)
        // - ISistemaApiService (gerenciamento de sistemas)
        // - IBancoApiService (gerenciamento de bancos)
        // Vantagens: Elimina duplicação, facilita adição de novos serviços
        builder.Services.AddApiServices(builder.Configuration);

        // Registra o HttpContextAccessor
        // Necessário para acessar o HttpContext em serviços que não são Controllers
        // (ex: BaseApiService precisa acessar o token JWT do usuário autenticado)
        // NOTA: Este registro já está incluído em AddApiServices, mas mantido aqui
        // para compatibilidade com TagHelpers e outros componentes que possam depender dele
        builder.Services.AddHttpContextAccessor();

        // ========================================
        // AUTENTICAÇÃO E AUTORIZAÇÃO
        // ========================================

        // Configura autenticação baseada em Cookies
        // A aplicação Web não usa JWT diretamente, mas armazena o token JWT
        // da API em um cookie seguro após o login
        builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                // Caminhos de redirecionamento para login, logout e acesso negado
                options.LoginPath = builder.Configuration["Authentication:LoginPath"] ?? "/Account/Login";
                options.LogoutPath = builder.Configuration["Authentication:LogoutPath"] ?? "/Account/Logout";
                options.AccessDeniedPath = builder.Configuration["Authentication:AccessDeniedPath"] ?? "/Account/AccessDenied";

                // Tempo de expiração do cookie (padrão: 480 minutos = 8 horas)
                options.ExpireTimeSpan = TimeSpan.FromMinutes(
                    builder.Configuration.GetValue<int>("Authentication:ExpireTimeSpan", 480));

                // SlidingExpiration: Renova o cookie automaticamente se o usuário estiver ativo
                options.SlidingExpiration = builder.Configuration.GetValue<bool>("Authentication:SlidingExpiration", true);

                // Configurações de segurança do cookie
                options.Cookie.HttpOnly = true; // Previne acesso via JavaScript (proteção contra XSS)
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Apenas HTTPS
                options.Cookie.SameSite = SameSiteMode.Lax; // Proteção contra CSRF
                options.Cookie.Name = builder.Configuration["Authentication:CookieName"] ?? "RhSensoERP.Auth";
            });

        // Registra o serviço de autorização (necessário para [Authorize] attributes)
        builder.Services.AddAuthorization();

        // ========================================
        // SESSÃO (STATE MANAGEMENT)
        // ========================================

        // Configura o serviço de sessão para armazenar dados temporários do usuário
        // Útil para armazenar mensagens de feedback (TempData), preferências, etc.
        builder.Services.AddSession(options =>
        {
            // Tempo de inatividade antes da sessão expirar (padrão: 30 minutos)
            options.IdleTimeout = TimeSpan.FromMinutes(30);

            // HttpOnly: Previne acesso via JavaScript
            options.Cookie.HttpOnly = true;

            // IsEssential: Cookie essencial para o funcionamento da aplicação
            // (não será bloqueado por políticas de consentimento de cookies)
            options.Cookie.IsEssential = true;
        });

        // ========================================
        // BUILD DA APLICAÇÃO
        // ========================================

        var app = builder.Build();

        // ========================================
        // PIPELINE DE REQUISIÇÕES (MIDDLEWARES)
        // ========================================
        // A ordem dos middlewares é CRÍTICA e deve ser respeitada:
        // 1. Exception Handling
        // 2. HTTPS Redirection
        // 3. Static Files
        // 4. Routing
        // 5. Session
        // 6. Authentication
        // 7. Authorization
        // 8. Endpoints (Controllers)

        // Tratamento de erros diferenciado por ambiente
        if (!app.Environment.IsDevelopment())
        {
            // Produção: Redireciona para página de erro genérica
            app.UseExceptionHandler("/Home/Error");

            // HSTS: HTTP Strict Transport Security
            // Força o navegador a usar HTTPS por um período determinado
            app.UseHsts();
        }
        // Em desenvolvimento, o DeveloperExceptionPage é habilitado automaticamente

        // Redireciona requisições HTTP para HTTPS
        app.UseHttpsRedirection();

        // Habilita o servidor de arquivos estáticos (CSS, JS, imagens)
        // Arquivos em wwwroot/ são servidos diretamente
        app.UseStaticFiles();

        // Habilita o roteamento de requisições
        app.UseRouting();

        // Habilita o middleware de sessão
        // Deve vir ANTES de Authentication/Authorization
        app.UseSession();

        // Habilita o middleware de autenticação
        // Popula o HttpContext.User com as claims do usuário autenticado
        app.UseAuthentication();

        // Habilita o middleware de autorização
        // Verifica se o usuário tem permissão para acessar o recurso solicitado
        app.UseAuthorization();

        // Define a rota padrão para os controllers
        // Padrão: /{controller=Home}/{action=Index}/{id?}
        // Exemplo: /Sistemas/Index/123
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        // ========================================
        // EXECUÇÃO DA APLICAÇÃO
        // ========================================

        try
        {
            Log.Information("🚀 Iniciando RhSensoERP.Web...");
            Log.Information("🌍 Ambiente: {Environment}", app.Environment.EnvironmentName);
            Log.Information("📍 URLs: {Urls}", string.Join(", ", app.Urls));

            // Inicia a aplicação e aguarda até que seja encerrada
            // ConfigureAwait(false): Libera o thread de sincronização, melhorando performance
            await app.RunAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Loga erros fatais que impedem a inicialização da aplicação
            Log.Fatal(ex, "💥 Erro fatal ao iniciar a aplicação");
            throw;
        }
        finally
        {
            // Garante que todos os logs pendentes sejam gravados antes de encerrar
            Log.Information("🛑 Encerrando RhSensoERP.Web...");
            await Log.CloseAndFlushAsync().ConfigureAwait(false);
        }
    }
}
