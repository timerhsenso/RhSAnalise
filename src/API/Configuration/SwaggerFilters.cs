// src/API/Configuration/SwaggerFilters.cs
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;

namespace RhSensoERP.API.Configuration;

/// <summary>
/// Filtro para adicionar valores padrão aos parâmetros do Swagger
/// </summary>
public class SwaggerDefaultValuesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;

        operation.Deprecated |= apiDescription.IsDeprecated();

        if (operation.Parameters == null)
            return;

        foreach (var parameter in operation.Parameters)
        {
            var description = apiDescription.ParameterDescriptions
                .First(p => p.Name == parameter.Name);

            parameter.Description ??= description.ModelMetadata?.Description;

            if (parameter.Schema.Default == null &&
                description.DefaultValue != null &&
                description.DefaultValue.ToString() != "" &&
                description.ModelMetadata != null)
            {
                var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata.ModelType);
                parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
            }

            parameter.Required |= description.IsRequired;
        }
    }
}

/// <summary>
/// Filtro para converter paths do Swagger para lowercase
/// </summary>
public class LowercaseDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var paths = new OpenApiPaths();

        foreach (var path in swaggerDoc.Paths)
        {
            paths.Add(path.Key.ToLowerInvariant(), path.Value);
        }

        swaggerDoc.Paths = paths;
    }
}

internal static class ApiDescriptionExtensions
{
    public static bool IsDeprecated(this ApiDescription apiDescription)
    {
        return apiDescription.CustomAttributes()
            .Any(a => a.GetType() == typeof(ObsoleteAttribute));
    }
}