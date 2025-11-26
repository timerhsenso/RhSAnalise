// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE DTOs/REQUESTS
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class DtoTemplate
{
    private static readonly string[] BaseEntityProps = { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" };

    public static string GenerateReadDto(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - DTO de Leitura");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// DTO de leitura para {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class {entity.ClassName}Dto");
        sb.AppendLine("{");

        foreach (var prop in entity.ScalarProperties.Where(p => !p.IgnoreInAllDtos && !p.IgnoreInReadDto))
        {
            sb.AppendLine($"    /// <summary>{prop.DisplayName}</summary>");
            sb.AppendLine($"    public {prop.FullTypeName} {prop.Name} {{ get; set; }}{GetInit(prop)}");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string GenerateCreateRequest(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Request de Criação");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Request para criar {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Create{entity.ClassName}Request");
        sb.AppendLine("{");

        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInCreateDto && !p.IsReadOnly && !IsBaseEntity(p.Name)))
        {
            sb.AppendLine($"    /// <summary>{prop.DisplayName}</summary>");
            AppendValidation(sb, prop);
            sb.AppendLine($"    public {prop.FullTypeName} {prop.Name} {{ get; set; }}{GetInit(prop)}");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static string GenerateUpdateRequest(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Request de Atualização");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using System.ComponentModel.DataAnnotations;");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Request para atualizar {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Update{entity.ClassName}Request");
        sb.AppendLine("{");

        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInUpdateDto && !p.IsReadOnly && !p.IsKey && !IsBaseEntity(p.Name)))
        {
            sb.AppendLine($"    /// <summary>{prop.DisplayName}</summary>");
            AppendValidation(sb, prop);
            sb.AppendLine($"    public {prop.FullTypeName} {prop.Name} {{ get; set; }}{GetInit(prop)}");
            sb.AppendLine();
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    private static bool IsBaseEntity(string name) => BaseEntityProps.Contains(name);

    private static string GetInit(PropertyInfo prop)
    {
        if (prop.IsNullable) return "";
        return prop.TypeName == "string" ? " = string.Empty;" : "";
    }

    private static void AppendValidation(StringBuilder sb, PropertyInfo prop)
    {
        if (prop.IsRequired && !prop.IsNullable)
        {
            var msg = prop.Messages.RequiredMessage ?? $"O campo {prop.DisplayName} é obrigatório.";
            sb.AppendLine($"    [Required(ErrorMessage = \"{Escape(msg)}\")]");
        }

        if (prop.MaxLength.HasValue && prop.IsString)
        {
            if (prop.MinLength.HasValue)
            {
                var msg = prop.Messages.LengthMessage ?? 
                    $"O campo {prop.DisplayName} deve ter entre {prop.MinLength} e {prop.MaxLength} caracteres.";
                sb.AppendLine($"    [StringLength({prop.MaxLength}, MinimumLength = {prop.MinLength}, ErrorMessage = \"{Escape(msg)}\")]");
            }
            else
            {
                var msg = prop.Messages.LengthMessage ?? 
                    $"O campo {prop.DisplayName} deve ter no máximo {prop.MaxLength} caracteres.";
                sb.AppendLine($"    [StringLength({prop.MaxLength}, ErrorMessage = \"{Escape(msg)}\")]");
            }
        }
    }

    private static string Escape(string s) => s.Replace("\"", "\\\"");
}
