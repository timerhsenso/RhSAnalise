// =============================================================================
// RhSensoERP - Módulo GestaoDeTerceiros - Dependency Injection
// =============================================================================
// Arquivo: src/Modules/GestaoDeTerceiros/DependencyInjection.cs
// Registra DbContext, AutoMapper, Repositórios e MediatR handlers
// =============================================================================
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;
using RhSensoERP.Modules.GestaoDeTerceiros.Infrastructure.Persistence;
using RhSensoERP.Modules.GestaoDeTerceiros.Infrastructure.Persistence.Contexts;


namespace RhSensoERP.Modules.GestaoDeTerceiros;

/// <summary>
/// Extensões de DI para o módulo GestaoDeTerceiros.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adiciona os serviços do módulo GestaoDeTerceiros ao container de DI.
    /// </summary>
    public static IServiceCollection AddGestaoDeTerceirosModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Assembly onde entidades e arquivos gerados estão
        var moduleAssembly = typeof(DependencyInjection).Assembly;

        // =====================================================================
        // 1. DbContext
        // =====================================================================
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<GestaoDeTerceirosDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(GestaoDeTerceirosDbContext).Assembly.FullName);
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
        // O Source Generator cria arquivos como BancoProfile.g.cs automaticamente
        // Basta escanear o assembly para encontrá-los
        services.AddAutoMapper(moduleAssembly);

        // =====================================================================
        // 3. Repositórios Gerados Automaticamente
        // =====================================================================
        services.AddGestaoDeTerceirosRepositories();

        // =====================================================================
        // 4. MediatR - Handlers do módulo (Commands, Queries)
        // =====================================================================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(moduleAssembly);
        });

        return services;
    }

    /// <summary>
    /// Registra automaticamente todos os repositórios gerados pelo Source Generator.
    /// </summary>
    public static IServiceCollection AddGestaoDeTerceirosRepositories(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
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