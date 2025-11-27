// =============================================================================
// RHSENSOERP - ATRIBUTOS PARA SOURCE GENERATOR
// =============================================================================
// COPIE ESTE ARQUIVO PARA: src/Shared/RhSensoERP.Shared.Core/Attributes/
// =============================================================================

namespace RhSensoERP.Shared.Core.Attributes;

/// <summary>
/// Atributo que marca uma Entity para geração automática de código CRUD.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateCrudAttribute : Attribute
{
    /// <summary>
    /// Nome da tabela no banco de dados.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Schema da tabela (padrão: "dbo").
    /// </summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>
    /// Nome amigável da entidade para exibição.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Código do sistema/módulo (SEG, RHU, FIN...).
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função/tela para permissões.
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    /// <summary>
    /// Rota base da API.
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Nome do grupo no Swagger.
    /// </summary>
    public string ApiGroup { get; set; } = string.Empty;

    // Flags Backend
    public bool GenerateDto { get; set; } = true;
    public bool GenerateRequests { get; set; } = true;
    public bool GenerateCommands { get; set; } = true;
    public bool GenerateQueries { get; set; } = true;
    public bool GenerateValidators { get; set; } = true;
    public bool GenerateRepository { get; set; } = true;
    public bool GenerateMapper { get; set; } = true;
    public bool GenerateEfConfig { get; set; } = true;

    // Flags Web/API (FALSE por padrão!)
    public bool GenerateApiController { get; set; } = false;
    public bool GenerateWebController { get; set; } = false;
    public bool GenerateWebModels { get; set; } = false;
    public bool GenerateWebServices { get; set; } = false;

    public bool ApiRequiresAuth { get; set; } = true;
    public bool SupportsBatchDelete { get; set; } = true;

    /// <summary>
    /// Indica se é tabela legada (sem BaseEntity).
    /// </summary>
    public bool IsLegacyTable { get; set; } = false;

    public string PrimaryKeyProperty { get; set; } = string.Empty;
    public string PrimaryKeyType { get; set; } = string.Empty;
}

/// <summary>
/// Define nome de exibição de um campo.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldDisplayNameAttribute : Attribute
{
    public string DisplayName { get; }
    public FieldDisplayNameAttribute(string displayName) => DisplayName = displayName;
}

/// <summary>
/// Marca campo para não incluir no DTO de leitura.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ExcludeFromDtoAttribute : Attribute { }

/// <summary>
/// Marca campo como somente leitura.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ReadOnlyFieldAttribute : Attribute { }

/// <summary>
/// Marca campo como obrigatório na criação.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class RequiredOnCreateAttribute : Attribute { }
