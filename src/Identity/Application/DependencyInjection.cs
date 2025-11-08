using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Identity.Application.Behaviors;
using RhSensoERP.Identity.Application.Mapping;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Application.Validators.Sistema;
using RhSensoERP.Identity.Core.Interfaces.Repositories;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Identity.Infrastructure.Repositories;
using System.Reflection;

namespace RhSensoERP.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // ==================== DATABASE ====================
        services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure();
                    sql.CommandTimeout(60);
                    sql.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName);
                });

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        });

        // ==================== MEDIATR ====================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // ==================== FLUENT VALIDATION ====================
        services.AddValidatorsFromAssembly(assembly);

        // ==================== AUTOMAPPER ====================
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UsuarioProfile>();
            cfg.AddProfile<PermissaoProfile>();
            cfg.AddProfile<SistemaProfile>();
        }, assembly);

        // ==================== REPOSITORIES ====================
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IPermissaoRepository, PermissaoRepository>();
        services.AddScoped<ISistemaRepository, SistemaRepository>();

        // ==================== SERVICES ====================
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPermissaoService, PermissaoService>();

        return services;
    }
}