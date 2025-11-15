using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Identity.Infrastructure.Repositories;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==================== DBCONTEXT ====================
        services.AddDbContext<IdentityDbContext>((serviceProvider, options) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure(3);
                sql.CommandTimeout(60);
            });

            // ✅ INTERCEPTORES (ordem importa!)
            var auditInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            var sqlLoggingInterceptor = serviceProvider.GetRequiredService<SqlLoggingInterceptor>();

            options.AddInterceptors(auditInterceptor, sqlLoggingInterceptor);

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });

        // ==================== REPOSITORIES ====================
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPermissaoRepository, PermissaoRepository>();
        services.AddScoped<ISistemaRepository, SistemaRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<IdentityDbContext>());

        return services;
    }
}