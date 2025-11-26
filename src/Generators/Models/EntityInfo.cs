// =============================================================================
// RHSENSOERP SOURCE GENERATOR - MODELS
// =============================================================================

namespace RhSensoERP.Generators.Models;

/// <summary>
/// Informações extraídas de uma Entity para geração de código.
/// </summary>
public sealed class EntityInfo
{
    /// <summary>Nome da classe (ex: "Sistema").</summary>
    public string ClassName { get; set; } = string.Empty;

    /// <summary>Namespace completo da classe.</summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>Nome de exibição para mensagens (ex: "Sistema").</summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Nome no plural (ex: "Sistemas").</summary>
    public string PluralName { get; set; } = string.Empty;

    /// <summary>Nome em camelCase (ex: "sistema").</summary>
    public string CamelCaseName { get; set; } = string.Empty;

    /// <summary>Nome da tabela no banco.</summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>Schema do banco (ex: "dbo").</summary>
    public string? Schema { get; set; }

    /// <summary>Lista de propriedades da Entity.</summary>
    public List<PropertyInfo> Properties { get; set; } = new();

    /// <summary>Propriedade que é a chave primária.</summary>
    public PropertyInfo? PrimaryKey => Properties.FirstOrDefault(p => p.IsKey);

    /// <summary>Propriedades que não são navegação.</summary>
    public IEnumerable<PropertyInfo> ScalarProperties => Properties.Where(p => !p.IsNavigation);

    /// <summary>Configurações do atributo [GenerateCrud].</summary>
    public GenerationConfig Config { get; set; } = new();

    // =========================================================================
    // NAMESPACES (Padrão RhSensoERP)
    // =========================================================================

    /// <summary>Namespace base.</summary>
    public string BaseNamespace => Config.BaseNamespace ?? GetBaseNamespaceFromEntity();

    /// <summary>Namespace para DTOs/Requests.</summary>
    public string DtoNamespace => Config.DtoNamespace ?? $"{BaseNamespace}.Application.DTOs.{ClassName}";

    /// <summary>Namespace para Commands.</summary>
    public string CommandsNamespace => Config.CommandsNamespace ?? $"{BaseNamespace}.Application.Features.{ClassName}.Commands";

    /// <summary>Namespace para Queries.</summary>
    public string QueriesNamespace => Config.QueriesNamespace ?? $"{BaseNamespace}.Application.Features.{ClassName}.Queries";

    /// <summary>Namespace para Validators.</summary>
    public string ValidatorsNamespace => Config.ValidatorsNamespace ?? $"{BaseNamespace}.Application.Validators.{ClassName}";

    /// <summary>Namespace para Repository Interface.</summary>
    public string RepositoryInterfaceNamespace => $"{BaseNamespace}.Core.Interfaces.Repositories";

    /// <summary>Namespace para Repository Implementation.</summary>
    public string RepositoryImplementationNamespace => Config.RepositoryNamespace ?? $"{BaseNamespace}.Infrastructure.Repositories";

    /// <summary>Namespace para AutoMapper Profile.</summary>
    public string MapperNamespace => $"{BaseNamespace}.Application.Mapping";

    /// <summary>Namespace para EF Configuration.</summary>
    public string ConfigurationNamespace => Config.ConfigurationNamespace ?? $"{BaseNamespace}.Infrastructure.Persistence.Configurations";

    /// <summary>Namespace do DbContext.</summary>
    public string DbContextNamespace => $"{BaseNamespace}.Infrastructure.Persistence";

    /// <summary>Nome do DbContext.</summary>
    public string DbContextName => GetDbContextName();

    private string GetBaseNamespaceFromEntity()
    {
        var ns = Namespace;
        var suffixes = new[] { ".Domain.Entities", ".Entities", ".Domain" };
        foreach (var suffix in suffixes)
        {
            if (ns.EndsWith(suffix))
                return ns.Substring(0, ns.Length - suffix.Length);
        }
        return ns;
    }

    private string GetDbContextName()
    {
        // RhSensoERP.Identity -> IdentityDbContext
        var parts = BaseNamespace.Split('.');
        var last = parts.LastOrDefault() ?? "App";
        return $"{last}DbContext";
    }
}

/// <summary>
/// Informações de uma propriedade da Entity.
/// </summary>
public sealed class PropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public string FullTypeName => IsNullable && !TypeName.EndsWith("?") ? $"{TypeName}?" : TypeName;

    // Banco de dados
    public string ColumnName { get; set; } = string.Empty;
    public bool IsKey { get; set; }

    // Validações
    public bool IsRequired { get; set; }
    public int? MaxLength { get; set; }
    public int? MinLength { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
    public string? Pattern { get; set; }
    public ValidationMessages Messages { get; set; } = new();

    // Geração
    public bool IgnoreInAllDtos { get; set; }
    public bool IgnoreInCreateDto { get; set; }
    public bool IgnoreInUpdateDto { get; set; }
    public bool IgnoreInReadDto { get; set; }
    public bool IsReadOnly { get; set; }
    public bool IsNavigation { get; set; }

    // Helpers
    public bool IsString => TypeName == "string" || TypeName == "String";
    public bool IsNumeric => TypeName is "int" or "long" or "decimal" or "double" or "float" or "short" or "byte";
    public bool IsBoolean => TypeName is "bool" or "Boolean";
    public bool IsDateTime => TypeName is "DateTime" or "DateTimeOffset";
}

public sealed class ValidationMessages
{
    public string? RequiredMessage { get; set; }
    public string? LengthMessage { get; set; }
    public string? RangeMessage { get; set; }
    public string? PatternMessage { get; set; }
}

public sealed class GenerationConfig
{
    public bool GenerateAsPartial { get; set; } = false; // Não usar partial por padrão

    // DTOs/Requests
    public bool GenerateDto { get; set; } = true;
    public bool GenerateCreateDto { get; set; } = true;
    public bool GenerateUpdateDto { get; set; } = true;

    // Commands
    public bool GenerateCreateCommand { get; set; } = true;
    public bool GenerateUpdateCommand { get; set; } = true;
    public bool GenerateDeleteCommand { get; set; } = true;
    public bool GenerateDeleteBatchCommand { get; set; } = true;

    // Queries
    public bool GenerateGetByIdQuery { get; set; } = true;
    public bool GenerateGetPagedQuery { get; set; } = true;

    // Validators
    public bool GenerateCreateValidator { get; set; } = true;
    public bool GenerateUpdateValidator { get; set; } = true;

    // Repository
    public bool GenerateRepositoryInterface { get; set; } = true;
    public bool GenerateRepositoryImplementation { get; set; } = true;

    // Mapper & EF
    public bool GenerateMapperProfile { get; set; } = true;
    public bool GenerateEfConfiguration { get; set; } = true;

    // Banco
    public string? TableName { get; set; }
    public string? Schema { get; set; }

    // Namespaces customizados
    public string? BaseNamespace { get; set; }
    public string? DtoNamespace { get; set; }
    public string? CommandsNamespace { get; set; }
    public string? QueriesNamespace { get; set; }
    public string? ValidatorsNamespace { get; set; }
    public string? RepositoryNamespace { get; set; }
    public string? ConfigurationNamespace { get; set; }

    // Display
    public string? DisplayName { get; set; }
}
