// =============================================================================
// RHSENSOERP GENERATOR v3.0 - WEB MODELS TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Web Models (DTOs e ViewModels).
/// NOVO no v3.0!
/// </summary>
public static class WebModelsTemplate
{
    /// <summary>
    /// Gera o DTO de leitura do Web.
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

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para exibição de {{info.DisplayName}} no frontend.
/// </summary>
public sealed class {{info.EntityName}}Dto
{
{{props}}
}
""";
    }

    /// <summary>
    /// Gera o DTO de criação do Web.
    /// </summary>
    public static string GenerateCreateDto(EntityInfo info)
    {
        var props = string.Join("\n", info.CreateProperties.Select(p =>
            $"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}"));

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}Dto
{
{{props}}
}
""";
    }

    /// <summary>
    /// Gera o DTO de atualização do Web.
    /// </summary>
    public static string GenerateUpdateDto(EntityInfo info)
    {
        var props = string.Join("\n", info.UpdateProperties.Select(p =>
            $"    public {p.Type} {p.Name} {{ get; set; }}{GetDefaultValue(p)}"));

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================

namespace {{info.WebModelsNamespace}};

/// <summary>
/// DTO para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}Dto
{
{{props}}
}
""";
    }

    /// <summary>
    /// Gera o ViewModel para listagem.
    /// </summary>
    public static string GenerateListViewModel(EntityInfo info)
    {
        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace {{info.WebModelsNamespace}};

/// <summary>
/// ViewModel para a página de listagem de {{info.DisplayName}}.
/// Contém informações de permissões do usuário para controle de botões.
/// </summary>
public sealed class {{info.PluralName}}ListViewModel : BaseListViewModel
{
    /// <summary>
    /// Título da página.
    /// </summary>
    public string PageTitle { get; set; } = "{{info.DisplayName}}";

    /// <summary>
    /// Subtítulo da página.
    /// </summary>
    public string PageSubtitle { get; set; } = "Gerenciamento de {{info.DisplayName}}";

    /// <summary>
    /// Código da função para permissões.
    /// </summary>
    public string CdFuncao { get; set; } = "{{info.CdFuncao}}";

    /// <summary>
    /// Código do sistema.
    /// </summary>
    public string CdSistema { get; set; } = "{{info.CdSistema}}";
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
