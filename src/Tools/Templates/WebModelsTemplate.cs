// =============================================================================
// RHSENSOERP CRUD TOOL - WEB MODELS TEMPLATE
// =============================================================================
using RhSensoERP.CrudTool.Models;

namespace RhSensoERP.CrudTool.Templates;

public static class WebModelsTemplate
{
    /// <summary>
    /// Gera o DTO de leitura completo.
    /// </summary>
    public static string GenerateDto(EntityConfig entity)
    {
        var properties = GenerateProperties(entity.Properties, includeAll: true);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.Text.Json.Serialization;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// DTO de leitura para {entity.DisplayName}.
/// </summary>
public class {entity.Name}Dto
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o DTO de criação.
    /// </summary>
    public static string GenerateCreateDto(EntityConfig entity)
    {
        // Propriedades para criação (exclui PK se for auto-gerada, inclui se for informada pelo usuário)
        var createProps = entity.Properties
            .Where(p => !p.IsReadOnly && (!p.IsPrimaryKey || entity.PrimaryKey.Type == "string"))
            .ToList();

        var properties = GeneratePropertiesWithValidation(createProps);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// DTO para criação de {entity.DisplayName}.
/// </summary>
public class Create{entity.Name}Dto
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o DTO de atualização.
    /// </summary>
    public static string GenerateUpdateDto(EntityConfig entity)
    {
        // Propriedades para atualização (exclui PK e readonly)
        var updateProps = entity.Properties
            .Where(p => !p.IsPrimaryKey && !p.IsReadOnly)
            .ToList();

        var properties = GeneratePropertiesWithValidation(updateProps);

        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// DTO para atualização de {entity.DisplayName}.
/// </summary>
public class Update{entity.Name}Dto
{{
{properties}
}}
";
    }

    /// <summary>
    /// Gera o ListViewModel.
    /// </summary>
    public static string GenerateListViewModel(EntityConfig entity)
    {
        return $@"// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// ViewModel para listagem de {entity.DisplayName}.
/// </summary>
public class {entity.PluralName}ListViewModel : BaseListViewModel
{{
    public {entity.PluralName}ListViewModel()
    {{
        InitializeDefaults(""{entity.PluralName}"", ""{entity.PluralName}"");
    }}

    /// <summary>
    /// Itens da listagem (para uso sem DataTables).
    /// </summary>
    public List<{entity.Name}Dto> Items {{ get; set; }} = new();
}}
";
    }

    #region Helper Methods

    private static string GenerateProperties(List<PropertyConfig> properties, bool includeAll)
    {
        var lines = new List<string>();

        foreach (var prop in properties)
        {
            // Display name como comentário
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                lines.Add($"    /// <summary>");
                lines.Add($"    /// {prop.DisplayName}");
                lines.Add($"    /// </summary>");
            }

            // JsonPropertyName para compatibilidade com API
            var jsonName = char.ToLower(prop.Name[0]) + prop.Name.Substring(1);
            lines.Add($"    [JsonPropertyName(\"{jsonName}\")]");

            // Propriedade com valor padrão
            lines.Add($"    {prop.GetPropertyDeclaration()}");
            lines.Add("");
        }

        return string.Join("\n", lines).TrimEnd();
    }

    private static string GeneratePropertiesWithValidation(List<PropertyConfig> properties)
    {
        var lines = new List<string>();

        foreach (var prop in properties)
        {
            // Display name
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                lines.Add($"    /// <summary>");
                lines.Add($"    /// {prop.DisplayName}");
                lines.Add($"    /// </summary>");
                lines.Add($"    [Display(Name = \"{prop.DisplayName}\")]");
            }

            // Required
            if (prop.Required)
            {
                var errorMsg = !string.IsNullOrEmpty(prop.DisplayName)
                    ? prop.DisplayName
                    : prop.Name;
                lines.Add($"    [Required(ErrorMessage = \"{errorMsg} é obrigatório\")]");
            }

            // StringLength
            if (prop.MaxLength.HasValue && prop.IsString)
            {
                if (prop.MinLength.HasValue)
                {
                    lines.Add($"    [StringLength({prop.MaxLength.Value}, MinimumLength = {prop.MinLength.Value}, ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter entre {prop.MinLength.Value} e {prop.MaxLength.Value} caracteres\")]");
                }
                else
                {
                    lines.Add($"    [StringLength({prop.MaxLength.Value}, ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter no máximo {prop.MaxLength.Value} caracteres\")]");
                }
            }

            // Propriedade com valor padrão
            lines.Add($"    {prop.GetPropertyDeclaration()}");
            lines.Add("");
        }

        return string.Join("\n", lines).TrimEnd();
    }

    #endregion
}