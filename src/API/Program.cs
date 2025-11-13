// src/API/Program.cs
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RhSensoERP.API.Configuration;
using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Infrastructure;
using RhSensoERP.Modules.GestaoDePessoas;
using RhSensoERP.Shared.Infrastructure;
using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Text.Json.Serialization;

namespace RhSensoERP.API;

/// <summary>
/// Classe principal responsável por inicializar e configurar a API RhSensoERP.
/// </summary>
public static class Program
{
    public static async Task Main(string[] args)
    {
        // Configuração inicial do Serilog
        ConfigureBootstrapLogger();

        try
        {
            Log.Information("=== Iniciando RhSensoERP API v2.0 ===");
            Log.Information("Ambiente: {Environment}", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

            var builder = WebApplication.CreateBuilder(args);

            // Configuração avançada
            ConfigureBuilder(builder);

            var app = builder.Build();

            // Pipeline de requisições
            ConfigurePipeline(app);

            Log.Information("✅ Aplicação iniciada com sucesso");
            await app.RunAsync();
        }
        catch (Exception ex) when (ex is not HostAbortedException)
        {
            Log.Fatal(ex, "❌ Aplicação falhou ao iniciar");
            throw;
        }
        finally
        {
            Log.Information("=== Encerrando aplicação ===");
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureBootstrapLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production")
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                "logs/bootstrap-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateBootstrapLogger();
    }

    private static void ConfigureBuilder(WebApplicationBuilder builder)
    {
        // Configuração de ambiente
        ConfigureConfiguration(builder);

        // Serilog completo
        ConfigureSerilog(builder);

        // Serviços da aplicação
        ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

        // Configurações do servidor
        ConfigureKestrel(builder);
    }

    private static void ConfigureConfiguration(WebApplicationBuilder builder)
    {
        builder.Configuration
            .SetBasePath(builder.Environment.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables("RHSENSO_");

        // User Secrets apenas em desenvolvimento
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<SecretsConfig>();
            Log.Information("📦 User Secrets carregados");
        }

        // Azure Key Vault em Staging/Production
        if (builder.Environment.IsStaging() || builder.Environment.IsProduction())
        {
            var keyVaultUrl = builder.Configuration["AzureKeyVault:Url"];
            if (!string.IsNullOrEmpty(keyVaultUrl))
            {
                Log.Information("🔐 Conectando ao Azure Key Vault: {Url}", keyVaultUrl);
            }
        }
    }

    private static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore",
                context.HostingEnvironment.IsDevelopment() ? LogEventLevel.Debug : LogEventLevel.Warning)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentUserName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "RhSensoERP-API")
            .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
            .Enrich.WithProperty("Version", Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2.0.0")
            .WriteTo.Console()
            .WriteTo.File(
                path: builder.Configuration["Diagnostics:LogFilePath"] ?? "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                fileSizeLimitBytes: 10485760,
                rollOnFileSizeLimit: true));
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        // Módulos de infraestrutura
        services.AddSharedInfrastructure();
        services.AddIdentityApplication();
        services.AddIdentityInfrastructure(configuration);
        services.AddGestaoDePessoasModule(configuration);

        // ⭐ CRÍTICO: Aplicar ModuleGroupConvention ANTES de AddControllers
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new ConsumesAttribute("application/json"));

            // ✅ REGISTRA A CONVENTION PARA AUTO-ATRIBUIR GroupName
            options.Conventions.Add(new ModuleGroupConvention());
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        // API Explorer
        services.AddEndpointsApiExplorer();

        // Versionamento de API
        ConfigureApiVersioning(services);

        // ✅ Swagger usando SwaggerConfiguration.cs
        if (configuration.GetValue<bool>("Features:EnableSwagger"))
        {
            services.AddSwaggerDocs(); // Método do SwaggerConfiguration.cs
            Log.Information("✅ Swagger configurado com documentos por módulo");
        }

        // CORS
        ConfigureCors(services, configuration);

        // Rate Limiting
        if (configuration.GetValue<bool>("Features:EnableRateLimiting"))
        {
            ConfigureRateLimiting(services, configuration);
        }

        // Health Checks
        ConfigureHealthChecks(services, configuration);

        // Compressão
        if (configuration.GetValue<bool>("Features:EnableCompression"))
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });
        }

        // Headers de segurança
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(configuration.GetValue<int>("Security:HstsMaxAge", 365));
        });

        // Forwarded Headers para proxy reverso
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
    }

    private static void ConfigureApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-API-Version"),
                new MediaTypeApiVersionReader("ver"));
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
    }

    private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var corsOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy("DefaultPolicy", policy =>
            {
                if (corsOrigins.Length > 0)
                {
                    policy.WithOrigins(corsOrigins);
                }
                else
                {
                    policy.AllowAnyOrigin();
                }

                policy.AllowAnyMethod()
                      .AllowAnyHeader();

                if (configuration.GetValue<bool>("Cors:AllowCredentials"))
                {
                    policy.AllowCredentials();
                }

                var maxAge = configuration.GetValue<int?>("Cors:MaxAge");
                if (maxAge.HasValue)
                {
                    policy.SetPreflightMaxAge(TimeSpan.FromSeconds(maxAge.Value));
                }
            });
        });
    }

    private static void ConfigureRateLimiting(IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("RateLimit"));
        services.Configure<IpRateLimitPolicies>(configuration.GetSection("RateLimit:IpPolicies"));
        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }

    private static void ConfigureHealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        var healthChecksBuilder = services.AddHealthChecks();

        // Self check
        healthChecksBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });

        // SQL Server
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (!string.IsNullOrEmpty(connectionString))
        {
            healthChecksBuilder.AddSqlServer(
                connectionString,
                name: "sqlserver",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "ready", "database" });
        }

        // Redis
        var redisConfig = configuration["Redis:Configuration"];
        if (!string.IsNullOrEmpty(redisConfig))
        {
            healthChecksBuilder.AddRedis(
                redisConfig,
                name: "redis",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "ready", "cache" });
        }

        // Health Check UI
        if (configuration.GetValue<bool>("Features:EnableHealthCheckUI"))
        {
            services.AddHealthChecksUI(options =>
            {
                options.SetEvaluationTimeInSeconds(configuration.GetValue<int>("HealthChecks:UI:EvaluationTimeInSeconds", 30));
                options.MaximumHistoryEntriesPerEndpoint(configuration.GetValue<int>("HealthChecks:UI:MaximumHistoryEntriesPerEndpoint", 50));
                options.AddHealthCheckEndpoint("API Health", "/health");
            })
            .AddInMemoryStorage();
        }
    }

    private static void ConfigureKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = builder.Configuration.GetValue<long>("Performance:MaxRequestBodySize", 10485760);
            options.Limits.MaxConcurrentConnections = builder.Configuration.GetValue<int>("Performance:MaxConcurrentRequests", 100);
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("Performance:RequestTimeout", 300));
            options.AddServerHeader = false;
        });
    }

    private static void ConfigurePipeline(WebApplication app)
    {
        // Forwarded Headers (deve ser o primeiro)
        app.UseForwardedHeaders();

        // Tratamento de exceções
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        // Logging de requisições
        if (app.Configuration.GetValue<bool>("Features:EnableRequestLogging"))
        {
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondeu {StatusCode} em {Elapsed:0.0000}ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
                };
            });
        }

        // Segurança
        if (app.Configuration.GetValue<bool>("Security:RequireHttps"))
        {
            app.UseHttpsRedirection();
        }

        // Rate Limiting
        if (app.Configuration.GetValue<bool>("Features:EnableRateLimiting"))
        {
            app.UseIpRateLimiting();
        }

        // Compressão
        if (app.Configuration.GetValue<bool>("Features:EnableCompression"))
        {
            app.UseResponseCompression();
        }

        // CORS
        app.UseCors("DefaultPolicy");

        // ✅ Swagger usando SwaggerConfiguration.cs
        if (app.Configuration.GetValue<bool>("Features:EnableSwagger") &&
            (app.Environment.IsDevelopment() || app.Environment.IsStaging()))
        {
            app.UseSwaggerDocs(); // Método do SwaggerConfiguration.cs
            Log.Information("✅ Swagger UI disponível em /swagger");
        }

        // Autenticação e Autorização
        app.UseAuthentication();
        app.UseAuthorization();

        // Health Checks
        app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = _ => false
        });

        // Health Check UI
        if (app.Configuration.GetValue<bool>("Features:EnableHealthCheckUI"))
        {
            app.MapHealthChecksUI(options =>
            {
                options.UIPath = "/health-ui";
                options.ApiPath = "/health-api";
            });
        }

        // Controllers
        app.MapControllers();

        // Página inicial
        app.MapGet("/", () => Results.Ok(new
        {
            application = "RhSensoERP API",
            version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "2.0.0",
            environment = app.Environment.EnvironmentName,
            timestamp = DateTime.UtcNow,
            documentation = "/swagger"
        }));

        Log.Information("✅ Pipeline configurado com sucesso");
    }
}