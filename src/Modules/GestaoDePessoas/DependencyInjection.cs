// src/Modules/GestaoDePessoas/DependencyInjection.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using RhSensoERP.Modules.GestaoDePessoas.Application.Services;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Contexts;
using RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Repositories;

namespace RhSensoERP.Modules.GestaoDePessoas;

/// <summary>
/// Configuração de DI do módulo Gestão de Pessoas.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddGestaoDePessoasModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ==================== DbContext ====================
        services.AddDbContext<GestaoDePessoasDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                    sql.CommandTimeout(60);
                    sql.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
                });

#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine, LogLevel.Information);
#endif
        });

        // ==================== Repositórios ====================
      //  services.AddScoped<IMunicipioRepository, MunicipioRepository>();

        // ==================== Services - Cadastros Auxiliares ====================
     //   services.AddScoped<IBancoService, BancoService>();
        // services.AddScoped<IAgenciaService, AgenciaService>();
        // services.AddScoped<IMunicipioService, MunicipioService>();
        // services.AddScoped<ICargoService, CargoService>();
        // services.AddScoped<ICentroCustoService, CentroCustoService>();
        // services.AddScoped<IEmpresaService, EmpresaService>();
        // services.AddScoped<IFilialService, FilialService>();

        // ==================== Services - Funcionários ====================
        // services.AddScoped<IFuncionarioService, FuncionarioService>();

        return services;
    }
}