// =============================================================================
// RhSensoERP - Módulo Esocial - Dependency Injection
// =============================================================================
// Arquivo: src/Modules/Esocial/EsocialDependencyInjection.cs
// Registra DbContext, AutoMapper, Repositórios e MediatR handlers
// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Modules.Esocial.Infrastructure.Persistence.Contexts;
using RhSensoERP.Modules.Esocial.Infrastructure.Services;
using RhSensoERP.Shared.Contracts.Esocial.Interfaces;

namespace RhSensoERP.Modules.Esocial;

/// <summary>
/// Extensões de DI para o módulo Esocial.
/// </summary>
public static class EsocialDependencyInjection
{
    /// <summary>
    /// Adiciona os serviços do módulo Esocial ao container de DI.
    /// </summary>
    public static IServiceCollection AddEsocialModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Assembly onde entidades e arquivos gerados estão
        var moduleAssembly = typeof(EsocialDependencyInjection).Assembly;

        // =====================================================================
        // 1. DbContext
        // =====================================================================
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<EsocialDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(EsocialDbContext).Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });

            // Logging em desenvolvimento
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // =====================================================================
        // 2. AutoMapper - Escaneia Profiles GERADOS pelo Source Generator
        // =====================================================================
        services.AddAutoMapper(moduleAssembly);

        // =====================================================================
        // 3. Repositórios Gerados Automaticamente
        // =====================================================================
        services.AddEsocialRepositories();

        // =====================================================================
        // 4. MediatR - Handlers do módulo (Commands, Queries)
        // =====================================================================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(moduleAssembly);
        });

        // =====================================================================
        // 5. Serviços Compartilhados (Lookup Service)
        // =====================================================================
        services.AddScoped<IEsocialLookupService, EsocialLookupService>();

        return services;
    }

    /// <summary>
    /// Registra automaticamente todos os repositórios gerados pelo Source Generator.
    /// </summary>
    public static IServiceCollection AddEsocialRepositories(this IServiceCollection services)
    {
        var assembly = typeof(EsocialDependencyInjection).Assembly;
        var types = assembly.GetTypes();

        // Busca interfaces de repositório
        var repoInterfaces = types
            .Where(t => t.IsInterface
                     && t.Name.StartsWith("I")
                     && t.Name.EndsWith("Repository")
                     && t.Namespace?.Contains("Interfaces.Repositories") == true)
            .ToList();

        foreach (var interfaceType in repoInterfaces)
        {
            var implName = interfaceType.Name.Substring(1);

            var implType = types.FirstOrDefault(t =>
                t.IsClass
                && !t.IsAbstract
                && t.Name == implName
                && interfaceType.IsAssignableFrom(t));

            if (implType != null && !services.Any(sd => sd.ServiceType == interfaceType))
            {
                services.AddScoped(interfaceType, implType);
            }
        }

        return services;
    }
}