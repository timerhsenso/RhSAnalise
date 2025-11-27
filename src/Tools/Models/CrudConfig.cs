// =============================================================================
// RHSENSOERP CRUD TOOL - CONFIGURATION MODELS
// =============================================================================
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RhSensoERP.CrudTool.Models;

/// <summary>
/// Configuração raiz do arquivo crud-config.json
/// </summary>
public class CrudConfig
{
    [JsonPropertyName("solutionRoot")]
    public string SolutionRoot { get; set; } = ".";

    [JsonPropertyName("apiProject")]
    public string ApiProject { get; set; } = "src/RhSensoERP.API";

    [JsonPropertyName("webProject")]
    public string WebProject { get; set; } = "src/Web";

    [JsonPropertyName("entities")]
    public List<EntityConfig> Entities { get; set; } = new();
}

/// <summary>
/// Configuração de uma Entity para geração de CRUD
/// </summary>
public class EntityConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("pluralName")]
    public string PluralName { get; set; } = string.Empty;

    [JsonPropertyName("module")]
    public string Module { get; set; } = "Identity";

    [JsonPropertyName("tableName")]
    public string TableName { get; set; } = string.Empty;

    [JsonPropertyName("cdSistema")]
    public string CdSistema { get; set; } = string.Empty;

    [JsonPropertyName("cdFuncao")]
    public string CdFuncao { get; set; } = string.Empty;

    [JsonPropertyName("primaryKey")]
    public PrimaryKeyConfig PrimaryKey { get; set; } = new();

    [JsonPropertyName("properties")]
    public List<PropertyConfig> Properties { get; set; } = new();

    [JsonPropertyName("generate")]
    public GenerateConfig Generate { get; set; } = new();

    // Computed properties
    public string NameLower => Name.ToLower();
    public string PluralNameLower => PluralName.ToLower();
    public string ModuleLower => Module.ToLower();
}

/// <summary>
/// Configuração da chave primária
/// </summary>
public class PrimaryKeyConfig
{
    [JsonPropertyName("property")]
    public string Property { get; set; } = "Id";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

    [JsonPropertyName("column")]
    public string Column { get; set; } = "id";
}

/// <summary>
/// Configuração de uma propriedade
/// </summary>
public class PropertyConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "string";

    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    [JsonPropertyName("displayName")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonPropertyName("maxLength")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("minLength")]
    public int? MinLength { get; set; }

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("isPrimaryKey")]
    public bool IsPrimaryKey { get; set; }

    [JsonPropertyName("isReadOnly")]
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Valor padrão - aceita string, bool, int, etc.
    /// </summary>
    [JsonPropertyName("defaultValue")]
    public JsonElement? DefaultValue { get; set; }

    // Computed
    public bool IsNullable => Type.EndsWith("?");
    public string BaseType => Type.TrimEnd('?');
    public bool IsString => BaseType.Equals("string", StringComparison.OrdinalIgnoreCase);
    public bool IsBool => BaseType.Equals("bool", StringComparison.OrdinalIgnoreCase);
    public bool IsInt => BaseType.Equals("int", StringComparison.OrdinalIgnoreCase);
    public bool IsLong => BaseType.Equals("long", StringComparison.OrdinalIgnoreCase);
    public bool IsDecimal => BaseType.Equals("decimal", StringComparison.OrdinalIgnoreCase);
    public bool IsDateTime => BaseType.Equals("DateTime", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifica se há valor padrão definido
    /// </summary>
    public bool HasDefaultValue => DefaultValue.HasValue &&
                                   DefaultValue.Value.ValueKind != JsonValueKind.Null &&
                                   DefaultValue.Value.ValueKind != JsonValueKind.Undefined;

    /// <summary>
    /// Retorna o valor padrão formatado como código C#
    /// </summary>
    public string? GetDefaultValueAsCode()
    {
        if (!HasDefaultValue)
            return null;

        var value = DefaultValue!.Value;

        return value.ValueKind switch
        {
            JsonValueKind.String => $"\"{value.GetString()}\"",
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Number when IsInt => value.GetInt32().ToString(),
            JsonValueKind.Number when IsLong => $"{value.GetInt64()}L",
            JsonValueKind.Number when IsDecimal => $"{value.GetDecimal()}m",
            JsonValueKind.Number => value.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture),
            _ => value.GetRawText()
        };
    }

    /// <summary>
    /// Retorna a declaração completa da propriedade para DTOs
    /// Ex: "public bool Ativo { get; set; } = true;"
    /// </summary>
    public string GetPropertyDeclaration()
    {
        var defaultPart = HasDefaultValue ? $" = {GetDefaultValueAsCode()};" : ";";

        // Para strings sem default, usar string.Empty
        if (IsString && !HasDefaultValue && !IsNullable)
            defaultPart = " = string.Empty;";

        return $"public {Type} {Name} {{ get; set; }}{defaultPart}";
    }
}

/// <summary>
/// Flags de geração
/// </summary>
public class GenerateConfig
{
    [JsonPropertyName("apiController")]
    public bool ApiController { get; set; } = true;

    [JsonPropertyName("webController")]
    public bool WebController { get; set; } = true;

    [JsonPropertyName("webModels")]
    public bool WebModels { get; set; } = true;

    [JsonPropertyName("webServices")]
    public bool WebServices { get; set; } = true;

    [JsonPropertyName("view")]
    public bool View { get; set; } = false;

    [JsonPropertyName("javascript")]
    public bool JavaScript { get; set; } = false;
}