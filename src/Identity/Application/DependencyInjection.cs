using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RhSensoERP.Shared.Application.Behaviors;
using RhSensoERP.Identity.Application.Mapping;
using RhSensoERP.Identity.Application.Services;
using System.Reflection;

namespace RhSensoERP.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // ==================== MEDIATR ====================
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        // ==================== FLUENT VALIDATION ====================
        services.AddValidatorsFromAssembly(assembly);

        // ==================== AUTOMAPPER ====================
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UsuarioProfile>();
            cfg.AddProfile<PermissaoProfile>();
            cfg.AddProfile<TsistemaProfile>();
            cfg.AddProfile<AuthProfile>();
        }, assembly);

        // ==================== APPLICATION SERVICES ====================
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IPermissaoService, PermissaoService>();
        services.AddScoped<IAuthService, bkpAuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}