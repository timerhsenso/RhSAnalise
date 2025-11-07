using Microsoft.EntityFrameworkCore;
using AutoMapper;
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Identity.Application.Mapping;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Infrastructure.Repositories;

namespace RhSensoERP.API;

/// <summary>
/// Classe principal da aplicação.
/// </summary>
public static class Program
{
    /// <summary>
    /// Ponto de entrada da aplicação.
    /// </summary>
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ===============================================
        // Infra: DbContext (Identity)
        // Requer "ConnectionStrings:DefaultConnection" no appsettings*.json
        // ===============================================
        builder.Services.AddDbContext<IdentityDbContext>(options =>
        {
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"),
                sql =>
                {
                    sql.EnableRetryOnFailure();
                    sql.CommandTimeout(60);
                });

            // 🔥 Habilita logs detalhados das consultas EF Core
            options.EnableSensitiveDataLogging();      // Mostra valores dos parâmetros
            options.EnableDetailedErrors();            // Mostra mensagens completas
            options.LogTo(Console.WriteLine,           // Escreve no console
                LogLevel.Information);                 // Nível de log (pode usar Debug, Warning, etc.)
        });


        // ===============================================
        // AutoMapper (Profiles do módulo Identity)
        // ===============================================
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<UsuarioProfile>();
            cfg.AddProfile<PermissaoProfile>();
        });

        // ===============================================
        // DI: Repositórios e Serviços (Identity)
        // ===============================================
        builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<IPermissaoRepository, PermissaoRepository>();
        builder.Services.AddScoped<IPermissaoService, PermissaoService>();

        // ===============================================
        // MVC + Swagger
        // ===============================================
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // ===============================================
        // Pipeline HTTP
        // ===============================================
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // (Adicionar autenticação/autorizar quando implementar Identity/JWT)
        // app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync().ConfigureAwait(false);
    }
}
