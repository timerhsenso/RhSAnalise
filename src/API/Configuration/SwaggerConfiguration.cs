// src/API/Configuration/SwaggerConfiguration.cs
#nullable enable
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;

namespace RhSensoERP.API.Configuration;

public static class SwaggerConfiguration
{
    private static readonly (string Key, string Title)[] ModuleDocs =
    [
        ("Identity",           "Identity"),
        ("Diagnostics",        "Diagnostics"),
        ("GestaoDePessoas",    "Gestão de Pessoas"),
        ("ControleDePonto",    "Controle de Ponto"),
        ("Avaliacoes",         "Avaliações"),
        ("Esocial",            "eSocial"),
        ("SaudeOcupacional",   "Saúde Ocupacional"),
        ("Treinamentos",       "Treinamentos")
    ];

    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
        Log.Information("🔧 Configurando Swagger com {Count} módulos", ModuleDocs.Length);

        services.AddSwaggerGen(c =>
        {
            // ===== Documentos =====
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "RhSensoERP API - Todos os Módulos",
                Version = "v1",
                Description = "Documentação completa com todos os endpoints."
            });

            foreach (var (key, title) in ModuleDocs)
            {
                c.SwaggerDoc(key, new OpenApiInfo
                {
                    Title = title,
                    Version = "v1",
                    Description = $"Endpoints do módulo {title}."
                });
            }

            // ===== JWT =====
            var jwtScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Bearer {token}",
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            };
            c.AddSecurityDefinition("Bearer", jwtScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement { [jwtScheme] = Array.Empty<string>() });

            // ===== Configurações =====
            c.SupportNonNullableReferenceTypes();
            c.DescribeAllParametersInCamelCase();
            c.EnableAnnotations(); // ✅ IMPORTANTE: Habilita SwaggerTag
            c.UseInlineDefinitionsForEnums();
            c.CustomSchemaIds(t => t.FullName!.Replace("+", "."));

            // ===== XML Comments =====
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic))
            {
                var xml = Path.ChangeExtension(asm.Location, ".xml");
                if (File.Exists(xml))
                    c.IncludeXmlComments(xml, includeControllerXmlComments: true);
            }

            // ✅ CRÍTICO: TagActionsBy determina as SUBTAGS (Municipios, Bancos, etc)
            c.TagActionsBy(api =>
            {
                // 1. Tenta pegar descrição do SwaggerTag
                var swaggerTagAttr = api.ActionDescriptor.EndpointMetadata
                    .OfType<Swashbuckle.AspNetCore.Annotations.SwaggerOperationAttribute>()
                    .FirstOrDefault();

                // 2. Se não tem SwaggerTag, usa nome do controller
                var controllerName = api.ActionDescriptor.RouteValues["controller"];

                // 3. Retorna a tag apropriada
                // IMPORTANTE: Não usa GroupName aqui, pois GroupName é para determinar o DOCUMENTO
                if (!string.IsNullOrWhiteSpace(controllerName))
                {
                    return new[] { controllerName };
                }

                return new[] { "API" };
            });

            // ✅ CRÍTICO: DocInclusionPredicate determina qual DOCUMENTO (v1, GestaoDePessoas, etc)
            c.DocInclusionPredicate((docName, apiDesc) =>
            {
                // "v1" inclui TUDO
                if (docName == "v1")
                    return true;

                // Outros documentos: filtra por GroupName
                if (!string.IsNullOrWhiteSpace(apiDesc.GroupName))
                    return string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase);

                return false;
            });
        });

        Log.Information("✅ Swagger configurado com {Total} documentos", ModuleDocs.Length + 1);
        return services;
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(ui =>
        {
            // Documento geral
            ui.SwaggerEndpoint("/swagger/v1/swagger.json", "📚 Todos os Módulos");

            // Documentos por módulo
            foreach (var (key, title) in ModuleDocs)
            {
                ui.SwaggerEndpoint($"/swagger/{key}/swagger.json", title);
            }

            ui.RoutePrefix = "swagger";
            ui.DocumentTitle = "RhSensoERP API";
            ui.DocExpansion(DocExpansion.List); // ✅ Mostra subtags colapsadas
            ui.DefaultModelsExpandDepth(-1);
            ui.EnableDeepLinking();
            ui.EnableFilter();
            ui.DisplayOperationId();
            ui.DisplayRequestDuration();
        });

        Log.Information("✅ Swagger UI configurada");
        return app;
    }
}