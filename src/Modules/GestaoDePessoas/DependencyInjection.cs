// src/Modules/GestaoDePessoas/DependencyInjection.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RhSensoERP.Modules.GestaoDePessoas.Application.Services;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence;

namespace RhSensoERP.Modules.GestaoDePessoas;

public static class DependencyInjection
{
    public static IServiceCollection AddGestaoDePessoasModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<GestaoDePessoasContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(3);
                    sql.CommandTimeout(60);
                });

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        });

        // Services - Cadastros Auxiliares
        services.AddScoped<IBancoService, BancoService>();
        // services.AddScoped<IAgenciaService, AgenciaService>();
        // services.AddScoped<IMunicipioService, MunicipioService>();
        // services.AddScoped<ICargoService, CargoService>();
        // services.AddScoped<ICentroCustoService, CentroCustoService>();
        // services.AddScoped<IEmpresaService, EmpresaService>();
        // services.AddScoped<IFilialService, FilialService>();

        // Services - Funcionários
        // services.AddScoped<IFuncionarioService, FuncionarioService>();

        return services;
    }
}