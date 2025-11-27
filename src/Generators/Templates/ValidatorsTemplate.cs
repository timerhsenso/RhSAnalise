// =============================================================================
// RHSENSOERP GENERATOR v3.0 - VALIDATORS TEMPLATE
// =============================================================================
using RhSensoERP.Generators.Models;

namespace RhSensoERP.Generators.Templates;

/// <summary>
/// Template para geração de Validators (FluentValidation).
/// </summary>
public static class ValidatorsTemplate
{
    /// <summary>
    /// Gera o Validator para CreateRequest.
    /// </summary>
    public static string GenerateCreateValidator(EntityInfo info)
    {
        var rules = GenerateValidationRules(info.CreateProperties, info);

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using FluentValidation;
using {{info.DtoNamespace}};

namespace {{info.ValidatorsNamespace}};

/// <summary>
/// Validator para criação de {{info.DisplayName}}.
/// </summary>
public sealed class Create{{info.EntityName}}RequestValidator
    : AbstractValidator<Create{{info.EntityName}}Request>
{
    public Create{{info.EntityName}}RequestValidator()
    {
{{rules}}
    }
}
""";
    }

    /// <summary>
    /// Gera o Validator para UpdateRequest.
    /// </summary>
    public static string GenerateUpdateValidator(EntityInfo info)
    {
        var rules = GenerateValidationRules(info.UpdateProperties, info);

        return $$"""
// =============================================================================
// ARQUIVO GERADO AUTOMATICAMENTE - NÃO EDITAR MANUALMENTE
// Generator: RhSensoERP.Generators v3.0
// Entity: {{info.EntityName}}
// =============================================================================
using FluentValidation;
using {{info.DtoNamespace}};

namespace {{info.ValidatorsNamespace}};

/// <summary>
/// Validator para atualização de {{info.DisplayName}}.
/// </summary>
public sealed class Update{{info.EntityName}}RequestValidator
    : AbstractValidator<Update{{info.EntityName}}Request>
{
    public Update{{info.EntityName}}RequestValidator()
    {
{{rules}}
    }
}
""";
    }

    /// <summary>
    /// Gera as regras de validação para as propriedades.
    /// </summary>
    private static string GenerateValidationRules(
        IEnumerable<PropertyInfo> properties,
        EntityInfo info)
    {
        var rules = new List<string>();

        foreach (var prop in properties)
        {
            var ruleBuilder = new List<string>();

            // NotEmpty para campos obrigatórios
            if (prop.IsRequired || prop.RequiredOnCreate)
            {
                if (prop.IsString)
                    ruleBuilder.Add($".NotEmpty().WithMessage(\"{prop.DisplayName} é obrigatório\")");
                else
                    ruleBuilder.Add($".NotNull().WithMessage(\"{prop.DisplayName} é obrigatório\")");
            }

            // MaxLength para strings
            if (prop.IsString && prop.MaxLength.HasValue)
            {
                ruleBuilder.Add($".MaximumLength({prop.MaxLength.Value}).WithMessage(\"{prop.DisplayName} deve ter no máximo {prop.MaxLength.Value} caracteres\")");
            }

            // MinLength para strings
            if (prop.IsString && prop.MinLength.HasValue)
            {
                ruleBuilder.Add($".MinimumLength({prop.MinLength.Value}).WithMessage(\"{prop.DisplayName} deve ter no mínimo {prop.MinLength.Value} caracteres\")");
            }

            // Só adiciona se tem alguma regra
            if (ruleBuilder.Count > 0)
            {
                var rule = $"        RuleFor(x => x.{prop.Name})\n            {string.Join("\n            ", ruleBuilder)};";
                rules.Add(rule);
            }
        }

        return rules.Count > 0
            ? string.Join("\n\n", rules)
            : "        // Nenhuma regra de validação configurada";
    }
}
