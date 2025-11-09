using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Infrastructure;
using Serilog;

namespace RhSensoERP.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // Configurar Serilog
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando RhSensoERP API");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            // ==================== REGISTRAR CAMADAS DO IDENTITY ====================
            builder.Services.AddIdentityApplication();                      // MediatR, Validators, AutoMapper
            builder.Services.AddIdentityInfrastructure(builder.Configuration); // DbContext, Repositories

            // ==================== API ====================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "RhSensoERP API", Version = "v1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Aplicação falhou ao iniciar");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}