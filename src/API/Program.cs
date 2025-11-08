using RhSensoERP.Identity;
using RhSensoERP.Shared.Application.Behaviors;
using Serilog;

namespace RhSensoERP.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // ==================== CONFIGURAR SERILOG ====================
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando RhSensoERP API");

            var builder = WebApplication.CreateBuilder(args);

            // Usar Serilog
            builder.Host.UseSerilog();

            // ==================== REGISTRAR MÓDULO IDENTITY ====================
            // ISSO REGISTRA TUDO: MediatR, Validators, AutoMapper, DbContext, Repositories, Services
            builder.Services.AddIdentityModule(builder.Configuration);

            // ==================== MVC + SWAGGER ====================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "RhSensoERP API",
                    Version = "v1"
                });
            });

            // ==================== CORS ====================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // ==================== PIPELINE HTTP ====================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseCors("Default");
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
            Log.CloseAndFlush();
        }
    }
}