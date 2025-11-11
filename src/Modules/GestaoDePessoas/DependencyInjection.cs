using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Services - Cadastros Auxiliares
        services.AddScoped<IBancoService, BancoService>();
        // services.AddScoped<IAgenciaService, AgenciaService>();
        // services.AddScoped<IMunicipioService, MunicipioService>();
        // services.AddScoped<ICargoService, CargoService>();

        return services;
    }
}