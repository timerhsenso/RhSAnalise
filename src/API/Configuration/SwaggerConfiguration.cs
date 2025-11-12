// src/API/Configuration/SwaggerConfiguration.cs
#nullable enable
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace RhSensoERP.API.Configuration;

/// <summary>Configuração do Swagger/OpenAPI com múltiplos documentos por módulo + doc geral v1.</summary>
public static class SwaggerConfiguration
{
    // Defina aqui os módulos visíveis no combo
    private static readonly (string Key, string Title)[] ModuleDocs =
    [
        ("GestaoDePessoas",    "Gestão de Pessoas"),
        ("Identity",           "Identity"),
        ("Diagnostics",        "Diagnostics"),
        ("ControleDePonto",    "Controle de Ponto"),
        ("Avaliacoes",         "Avaliações"),
        ("Esocial",            "eSocial"),
        ("SaudeOcupacional",   "Saúde Ocupacional"),
        ("Treinamentos",       "Treinamentos")
    ];

    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // ===== Documento geral (agrega tudo) =====
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RhSensoERP API",
                Version = "v1",
                Description = "Doc geral (agrega todos os módulos)."
            });

            // ===== Um documento por módulo =====
            foreach (var (key, title) in ModuleDocs)
            {
                c.SwaggerDoc(key, new OpenApiInfo
                {
                    Title = $"{title} API",
                    Version = "v1",
                    Description = $"Endpoints do módulo {title}."
                });
            }

            // ==== Autenticação JWT Bearer ====
            var jwtScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Informe: Bearer {seu_token_jwt}",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            c.AddSecurityDefinition("Bearer", jwtScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { [jwtScheme] = Array.Empty<string>() });

            // ==== Boas práticas ====
            c.SupportNonNullableReferenceTypes();
            c.DescribeAllParametersInCamelCase();
            c.EnableAnnotations();
            c.UseInlineDefinitionsForEnums();

            // Crítico: evita conflito de schemas (vários módulos, muitos DTOs com mesmo nome)
            c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));

            // Comentários XML (se habilitados nos csproj)
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
            {
                var xml = Path.ChangeExtension(asm.Location, ".xml");
                if (File.Exists(xml))
                    c.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }

            // Agrupamento por GroupName quando existir; senão, por controller
            c.TagActionsBy(api =>
            {
                if (api.GroupName is not null) return new[] { api.GroupName };
                return new[] { api.ActionDescriptor.RouteValues["controller"] ?? "API" };
            });

            // Regras de inclusão:
            // - "v1" recebe tudo (doc geral)
            // - docs de módulo recebem apenas actions cujo GroupName == nome do doc
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                if (docName == "v1") return true; // geral
                return string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase);
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(ui =>
        {
            // Doc geral
            ui.SwaggerEndpoint("/swagger/v1/swagger.json", "RhSensoERP API (Geral)");

            // Docs por módulo (aparecem no combo)
            foreach (var (key, title) in ModuleDocs)
            {
                ui.SwaggerEndpoint($"/swagger/{key}/swagger.json", title);
            }

            ui.DisplayOperationId();
            ui.DisplayRequestDuration();
            ui.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        });

        return app;
    }
}
