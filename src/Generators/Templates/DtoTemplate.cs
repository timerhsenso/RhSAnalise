// =============================================================================
// RHSENSOERP GENERATOR v3.0 - DTO TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de DTOs (Data Transfer Objects).
/// </summary>
public static class DtoTemplate
{
    /// <summary>
    /// Gera o DTO de leitura (EntityDto.g.cs).
    /// </summary>
    public static string GenerateDto(EntityInfo info)
    {
        var props = string.Join("\n", info.DtoProperties.Select(p =>
            $"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}"));

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================

namespace {{info.DtoNamespace}};

/// <summary>
/// DTO para leitura de {{info.DisplayName}}.
/// </summary>
public sealed class {{info.EntityName}}Dto
{
{{props}}
}
""";
    }

    /// <summary>
    /// Gera o DTO de criação (CreateEntityRequest.g.cs).
    /// </summary>
    public static string GenerateCreateRequest(EntityInfo info)
    {
        var props = string.Join("\n", info.CreateProperties.Select(p =>
            $"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}"));

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================

namespace {{info.DtoNamespace}};

/// <summary>
/// Request para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}Request
{
{{props}}
}
""";
    }

    /// <summary>
    /// Gera o DTO de atualização (UpdateEntityRequest.g.cs).
    /// </summary>
    public static string GenerateUpdateRequest(EntityInfo info)
    {
        var props = string.Join("\n", info.UpdateProperties.Select(p =>
            $"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}"));

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================

namespace {{info.DtoNamespace}};

/// <summary>
/// Request para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}Request
{
{{props}}
}
""";
    }

    /// <summary>
    /// Obtém o valor padrão para uma propriedade.
    /// </summary>
    private static string GetDefaultValue(PropertyInfo prop)
    {
        if (!string.IsNullOrEmpty(prop.DefaultValue))
            return $" = {prop.DefaultValue};";

        if (prop.IsString && !prop.IsNullable)
            return " = string.Empty;";

        return string.Empty;
    }
}
