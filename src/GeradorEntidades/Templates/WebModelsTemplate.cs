// =============================================================================
// GERADOR FULL-STACK v3.0 - WEB MODELS TEMPLATE
// Migrado e adaptado de RhSensoERP.CrudTool
// =============================================================================

using GeradorEntidades.Models;
using System.Text;

namespace GeradorEntidades.Templates;

/// <summary>
/// Gera Models para o projeto Web (DTOs, Requests, ViewModels).
/// </summary>
public static class WebModelsTemplate
{
    /// <summary>
    /// Gera o DTO de leitura.
    /// </summary>
    public static GeneratedFile GenerateDto(EntityConfig entity)
    {
        var properties = GenerateProperties(entity.Properties);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// DTO de leitura para {entity.DisplayName}.
/// Compatível com backend: {entity.BackendNamespace ?? $"RhSensoERP.Modules.{entity.Module}.Application.DTOs.{entity.PluralName}"}.{entity.Name}Dto
/// </summary>
public class {entity.Name}Dto
{{
{properties}
}}
";

        return new GeneratedFile
        {
            FileName = $"{entity.Name}Dto.cs",
            RelativePath = $"Web/Models/{entity.PluralName}/{entity.Name}Dto.cs",
            Content = content,
            FileType = "Model"
        };
    }

    /// <summary>
    /// Gera o Request de criação.
    /// </summary>
    public static GeneratedFile GenerateCreateRequest(EntityConfig entity)
    {
        var createProps = entity.Properties
            .Where(p => !p.IsReadOnly && !p.IsPrimaryKey)
            .ToList();

        var properties = GeneratePropertiesWithValidation(createProps);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// Request para criação de {entity.DisplayName}.
/// Compatível com backend: Create{entity.Name}Request
/// </summary>
public class Create{entity.Name}Request
{{
{properties}
}}
";

        return new GeneratedFile
        {
            FileName = $"Create{entity.Name}Request.cs",
            RelativePath = $"Web/Models/{entity.PluralName}/Create{entity.Name}Request.cs",
            Content = content,
            FileType = "Model"
        };
    }

    /// <summary>
    /// Gera o Request de atualização.
    /// </summary>
    public static GeneratedFile GenerateUpdateRequest(EntityConfig entity)
    {
        var updateProps = entity.Properties
            .Where(p => !p.IsPrimaryKey && !p.IsReadOnly)
            .ToList();

        var properties = GeneratePropertiesWithValidation(updateProps);

        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// Request para atualização de {entity.DisplayName}.
/// Compatível com backend: Update{entity.Name}Request
/// </summary>
public class Update{entity.Name}Request
{{
{properties}
}}
";

        return new GeneratedFile
        {
            FileName = $"Update{entity.Name}Request.cs",
            RelativePath = $"Web/Models/{entity.PluralName}/Update{entity.Name}Request.cs",
            Content = content,
            FileType = "Model"
        };
    }

    /// <summary>
    /// Gera o ListViewModel que herda de BaseListViewModel.
    /// </summary>
    public static GeneratedFile GenerateListViewModel(EntityConfig entity)
    {
        var content = $@"// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: {entity.Name}
// Data: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.{entity.PluralName};

/// <summary>
/// ViewModel para listagem de {entity.DisplayName}.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class {entity.PluralName}ListViewModel : BaseListViewModel
{{
    public {entity.PluralName}ListViewModel()
    {{
        // Inicializa propriedades padrão
        InitializeDefaults(""{entity.PluralName}"", ""{entity.DisplayName}"");
        
        // Configurações específicas
        PageTitle = ""{entity.DisplayName}"";
        PageIcon = ""fas fa-list"";
        CdFuncao = ""{entity.CdFuncao}"";
    }}

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<{entity.Name}Dto> Items {{ get; set; }} = new();
}}
";

        return new GeneratedFile
        {
            FileName = $"{entity.PluralName}ListViewModel.cs",
            RelativePath = $"Web/Models/{entity.PluralName}/{entity.PluralName}ListViewModel.cs",
            Content = content,
            FileType = "Model"
        };
    }

    #region Helper Methods

    /// <summary>
    /// Gera propriedades sem validação (para DTOs de leitura).
    /// </summary>
    private static string GenerateProperties(List<PropertyConfig> properties)
    {
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            // Comentário XML com DisplayName
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {prop.DisplayName}");
                sb.AppendLine($"    /// </summary>");
            }

            // Propriedade
            sb.AppendLine($"    {prop.GetPropertyDeclaration()}");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// Gera propriedades com validação (para Requests de Create/Update).
    /// </summary>
    private static string GeneratePropertiesWithValidation(List<PropertyConfig> properties)
    {
        var sb = new StringBuilder();

        foreach (var prop in properties)
        {
            // Comentário XML
            if (!string.IsNullOrEmpty(prop.DisplayName))
            {
                sb.AppendLine($"    /// <summary>");
                sb.AppendLine($"    /// {prop.DisplayName}");
                sb.AppendLine($"    /// </summary>");
                sb.AppendLine($"    [Display(Name = \"{prop.DisplayName}\")]");
            }

            // Required
            if (prop.Required && !prop.IsNullable)
            {
                var errorMsg = !string.IsNullOrEmpty(prop.DisplayName)
                    ? prop.DisplayName
                    : prop.Name;
                sb.AppendLine($"    [Required(ErrorMessage = \"{errorMsg} é obrigatório\")]");
            }

            // StringLength
            if (prop.MaxLength.HasValue && prop.IsString)
            {
                if (prop.MinLength.HasValue && prop.MinLength.Value > 0)
                {
                    sb.AppendLine($"    [StringLength({prop.MaxLength.Value}, MinimumLength = {prop.MinLength.Value}, " +
                                 $"ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter entre {{2}} e {{1}} caracteres\")]");
                }
                else
                {
                    sb.AppendLine($"    [StringLength({prop.MaxLength.Value}, " +
                                 $"ErrorMessage = \"{prop.DisplayName ?? prop.Name} deve ter no máximo {{1}} caracteres\")]");
                }
            }

            // Propriedade
            sb.AppendLine($"    {prop.GetPropertyDeclaration()}");
            sb.AppendLine();
        }

        return sb.ToString().TrimEnd();
    }

    #endregion
}
