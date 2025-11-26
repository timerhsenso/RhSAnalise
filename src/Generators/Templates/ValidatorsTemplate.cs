// =============================================================================
// RHSENSOERP SOURCE GENERATOR - TEMPLATE DE VALIDATORS
// =============================================================================

using System.Text;
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

public static class ValidatorsTemplate
{
    private static readonly string[] BaseEntityProps = { "Id", "CreatedAt", "CreatedBy", "UpdatedAt", "UpdatedBy" };

    public static string GenerateCreateValidator(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Create Request Validator");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.ValidatorsNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Validador do request de criação de {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Create{entity.ClassName}RequestValidator : AbstractValidator<Create{entity.ClassName}Request>");
        sb.AppendLine("{");
        sb.AppendLine($"    public Create{entity.ClassName}RequestValidator()");
        sb.AppendLine("    {");

        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInCreateDto && !p.IsReadOnly && !IsBaseEntity(p.Name) &&
            (p.IsRequired || p.MaxLength.HasValue || p.MinLength.HasValue)))
        {
            AppendPropertyRules(sb, prop);
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    public static string GenerateUpdateValidator(EntityInfo entity)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// =============================================================================");
        sb.AppendLine("// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR!");
        sb.AppendLine($"// Entity: {entity.ClassName} - Update Request Validator");
        sb.AppendLine("// =============================================================================");
        sb.AppendLine();
        sb.AppendLine("using FluentValidation;");
        sb.AppendLine($"using {entity.DtoNamespace};");
        sb.AppendLine();
        sb.AppendLine($"namespace {entity.ValidatorsNamespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Validador do request de atualização de {entity.DisplayName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"public sealed class Update{entity.ClassName}RequestValidator : AbstractValidator<Update{entity.ClassName}Request>");
        sb.AppendLine("{");
        sb.AppendLine($"    public Update{entity.ClassName}RequestValidator()");
        sb.AppendLine("    {");

        foreach (var prop in entity.ScalarProperties.Where(p => 
            !p.IgnoreInAllDtos && !p.IgnoreInUpdateDto && !p.IsReadOnly && !p.IsKey && !IsBaseEntity(p.Name) &&
            (p.IsRequired || p.MaxLength.HasValue || p.MinLength.HasValue)))
        {
            AppendPropertyRules(sb, prop);
        }

        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void AppendPropertyRules(StringBuilder sb, PropertyInfo prop)
    {
        sb.AppendLine();
        sb.AppendLine($"        // {prop.DisplayName}");
        sb.Append($"        RuleFor(x => x.{prop.Name})");

        // NotEmpty para strings, NotNull para outros
        if (prop.IsRequired && !prop.IsNullable)
        {
            sb.AppendLine();
            if (prop.IsString)
            {
                var msg = prop.Messages.RequiredMessage ?? $"{prop.DisplayName} é obrigatório.";
                sb.Append($"            .NotEmpty().WithMessage(\"{Escape(msg)}\")");
            }
            else
            {
                var msg = prop.Messages.RequiredMessage ?? $"{prop.DisplayName} é obrigatório.";
                sb.Append($"            .NotNull().WithMessage(\"{Escape(msg)}\")");
            }
        }

        // Length
        if (prop.IsString)
        {
            if (prop.MaxLength.HasValue && prop.MinLength.HasValue)
            {
                var msg = prop.Messages.LengthMessage ?? 
                    $"{prop.DisplayName} deve ter entre {prop.MinLength} e {prop.MaxLength} caracteres.";
                sb.AppendLine();
                sb.Append($"            .Length({prop.MinLength}, {prop.MaxLength}).WithMessage(\"{Escape(msg)}\")");
            }
            else if (prop.MaxLength.HasValue)
            {
                var msg = prop.Messages.LengthMessage ?? 
                    $"{prop.DisplayName} deve ter no máximo {prop.MaxLength} caracteres.";
                sb.AppendLine();
                sb.Append($"            .MaximumLength({prop.MaxLength}).WithMessage(\"{Escape(msg)}\")");
            }
            else if (prop.MinLength.HasValue)
            {
                var msg = prop.Messages.LengthMessage ?? 
                    $"{prop.DisplayName} deve ter no mínimo {prop.MinLength} caracteres.";
                sb.AppendLine();
                sb.Append($"            .MinimumLength({prop.MinLength}).WithMessage(\"{Escape(msg)}\")");
            }
        }

        sb.AppendLine(";");
    }

    private static bool IsBaseEntity(string name) => BaseEntityProps.Contains(name);
    private static string Escape(string s) => s.Replace("\"", "\\\"");
}
