// src/API/Program.cs

using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Infrastructure;
using RhSensoERP.Modules.GestaoDePessoas;
using RhSensoERP.Shared.Infrastructure;
using Serilog;
using Microsoft.OpenApi.Models;

namespace RhSensoERP.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando RhSensoERP API");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            // ==================== SHARED INFRASTRUCTURE ====================
            builder.Services.AddSharedInfrastructure();

            // ==================== IDENTITY ====================
            builder.Services.AddIdentityApplication();
            builder.Services.AddIdentityInfrastructure(builder.Configuration);

            // ==================== MÓDULO GESTÃO DE PESSOAS ====================
            builder.Services.AddGestaoDePessoasModule(builder.Configuration);

            // ==================== API ====================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configuração do Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                // Documento principal
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "RhSensoERP API",
                    Version = "v1",
                    Description = "API do sistema RhSensoERP - Sistema de Gestão de Recursos Humanos",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipe RhSenso",
                        Email = "suporte@rhsenso.com.br"
                    }
                });

                // Grupo específico para Gestão de Pessoas
                options.SwaggerDoc("GestaoDePessoas", new OpenApiInfo
                {
                    Title = "Módulo Gestão de Pessoas",
                    Version = "v1",
                    Description = "APIs do módulo de Gestão de Pessoas - Cadastros e manutenção de funcionários"
                });

                // ✅ ADICIONAR FILTRO PARA SEPARAR OS GRUPOS
                options.DocInclusionPredicate((docName, apiDesc) =>
                {
                    if (docName == "v1")
                    {
                        // v1 inclui controllers SEM grupo OU com grupo "v1"
                        var groupName = apiDesc.GroupName;
                        return string.IsNullOrEmpty(groupName) || groupName == "v1";
                    }

                    // Outros documentos (GestaoDePessoas, etc.) filtram pelo nome do grupo
                    return apiDesc.GroupName == docName;
                });

                // Habilitar anotações do Swagger
                options.EnableAnnotations();

                // Configurar autorização JWT
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ==================== CORS ====================
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // ==================== PIPELINE ====================
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "RhSensoERP API v1");
                    c.SwaggerEndpoint("/swagger/GestaoDePessoas/swagger.json", "Gestão de Pessoas v1");
                    c.RoutePrefix = "swagger";
                    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                });
            }

            app.UseSerilogRequestLogging();
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            Log.Information("Aplicação iniciada com sucesso");
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Aplicação falhou ao iniciar");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}