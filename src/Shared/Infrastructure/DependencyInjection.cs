namespace RhSensoERP.Shared.Infrastructure;

using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;
using RhSensoERP.Shared.Infrastructure.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        // Services
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // EF Core Interceptors
        services.AddScoped<AuditableEntityInterceptor>();

        return services;
    }
}