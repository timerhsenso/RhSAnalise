// =============================================================================
// RHSENSOERP GENERATOR v3.0 - GENERATE CRUD ATTRIBUTE
// =============================================================================
// Arquivo: src/Generators/Attributes/GenerateCrudAttribute.cs
// Versão: 3.0 - Com suporte a CdSistema e CdFuncao para permissões
// =============================================================================

namespace RhSensoERP.Generators.Attributes;

/// <summary>
/// Atributo que marca uma Entity para geração automática de código CRUD.
/// Gera: DTOs, Commands, Queries, Validators, Repository, Mapper, EF Config,
/// API Controller, Web Controller, Web Models e Web Services.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class GenerateCrudAttribute : Attribute
{
    // =========================================================================
    // CONFIGURAÇÕES BÁSICAS
    // =========================================================================

    /// <summary>
    /// Nome da tabela no banco de dados.
    /// Exemplo: "tsistema", "tfunc1", "tusuario"
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Schema da tabela (padrão: "dbo").
    /// </summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>
    /// Nome amigável da entidade para exibição.
    /// Exemplo: "Sistema", "Função", "Usuário"
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÕES DE MÓDULO E PERMISSÕES (NOVO v3.0)
    // =========================================================================

    /// <summary>
    /// Código do sistema/módulo ao qual esta entidade pertence.
    /// Exemplos: "SEG" (Segurança), "RHU" (RH), "FIN" (Financeiro)
    /// Usado para: Agrupamento de API, rotas, logs.
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função/tela no sistema de permissões legado.
    /// Exemplo: "SEG_FM_TSISTEMA", "RHU_FM_FUNCIONARIO"
    /// Usado para: Verificação de permissões IAEC no Web Controller.
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÕES DE ROTA E API
    // =========================================================================

    /// <summary>
    /// Rota base da API (sem prefixo api/).
    /// Se vazio, será gerado automaticamente: {module}/{entityPlural}
    /// Exemplo: "identity/sistemas" → /api/identity/sistemas
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Nome do grupo no Swagger/OpenAPI.
    /// Se vazio, usa o CdSistema ou o módulo inferido.
    /// </summary>
    public string ApiGroup { get; set; } = string.Empty;

    // =========================================================================
    // FLAGS DE GERAÇÃO - BACKEND
    // =========================================================================

    /// <summary>
    /// Gera o DTO de leitura (EntityDto.g.cs).
    /// </summary>
    public bool GenerateDto { get; set; } = true;

    /// <summary>
    /// Gera os DTOs de request (CreateEntityRequest, UpdateEntityRequest).
    /// </summary>
    public bool GenerateRequests { get; set; } = true;

    /// <summary>
    /// Gera os Commands (Create, Update, Delete, DeleteMultiple).
    /// </summary>
    public bool GenerateCommands { get; set; } = true;

    /// <summary>
    /// Gera as Queries (GetById, GetPaged).
    /// </summary>
    public bool GenerateQueries { get; set; } = true;

    /// <summary>
    /// Gera os Validators (CreateRequest, UpdateRequest).
    /// </summary>
    public bool GenerateValidators { get; set; } = true;

    /// <summary>
    /// Gera a Interface e Implementação do Repository.
    /// </summary>
    public bool GenerateRepository { get; set; } = true;

    /// <summary>
    /// Gera o perfil do AutoMapper.
    /// </summary>
    public bool GenerateMapper { get; set; } = true;

    /// <summary>
    /// Gera a configuração do Entity Framework.
    /// </summary>
    public bool GenerateEfConfig { get; set; } = true;

    // =========================================================================
    // FLAGS DE GERAÇÃO - API CONTROLLER (NOVO v3.0)
    // =========================================================================

    /// <summary>
    /// Gera o API Controller (src/API/Controllers/{Module}/{Entity}Controller.cs).
    /// PADRÃO: false - Arquivos devem ser copiados manualmente para o projeto API.
    /// </summary>
    public bool GenerateApiController { get; set; } = false;

    /// <summary>
    /// Adiciona [Authorize] ao API Controller.
    /// </summary>
    public bool ApiRequiresAuth { get; set; } = true;

    // =========================================================================
    // FLAGS DE GERAÇÃO - WEB (NOVO v3.0)
    // =========================================================================

    /// <summary>
    /// Gera o Web Controller (src/Web/Controllers/{Entity}Controller.cs).
    /// PADRÃO: false - Arquivos devem ser copiados manualmente para o projeto Web.
    /// </summary>
    public bool GenerateWebController { get; set; } = false;

    /// <summary>
    /// Gera os Models do Web (DTOs e ViewModel).
    /// PADRÃO: false - Arquivos devem ser copiados manualmente para o projeto Web.
    /// </summary>
    public bool GenerateWebModels { get; set; } = false;

    /// <summary>
    /// Gera os Services do Web (Interface e Implementação).
    /// PADRÃO: false - Arquivos devem ser copiados manualmente para o projeto Web.
    /// </summary>
    public bool GenerateWebServices { get; set; } = false;

    // =========================================================================
    // FLAGS DE FUNCIONALIDADES
    // =========================================================================

    /// <summary>
    /// Gera operação de exclusão em lote (DeleteMultiple/Batch).
    /// </summary>
    public bool SupportsBatchDelete { get; set; } = true;

    /// <summary>
    /// Indica se é uma tabela legada (sem BaseEntity).
    /// Se true, não inclui colunas de auditoria (Id, CreatedAt, UpdatedAt, etc).
    /// </summary>
    public bool IsLegacyTable { get; set; } = false;

    /// <summary>
    /// Nome da propriedade da chave primária (se diferente de "Id").
    /// Usado para tabelas legadas com PK customizada.
    /// </summary>
    public string PrimaryKeyProperty { get; set; } = string.Empty;

    /// <summary>
    /// Tipo da chave primária (string, int, Guid).
    /// Se vazio, será inferido da propriedade [Key].
    /// </summary>
    public string PrimaryKeyType { get; set; } = string.Empty;
}

/// <summary>
/// Atributo para definir nome de exibição de campos.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FieldDisplayNameAttribute : Attribute
{
    public string DisplayName { get; }

    public FieldDisplayNameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}

/// <summary>
/// Atributo para marcar campos que não devem ser incluídos no DTO de leitura.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ExcludeFromDtoAttribute : Attribute { }

/// <summary>
/// Atributo para marcar campos que não devem ser editáveis (apenas leitura).
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ReadOnlyFieldAttribute : Attribute { }

/// <summary>
/// Atributo para marcar campos obrigatórios na criação.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class RequiredOnCreateAttribute : Attribute { }

/// <summary>
/// Atributo para configurar validação customizada.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
public sealed class ValidationRuleAttribute : Attribute
{
    public string RuleType { get; }
    public string? Parameter { get; set; }
    public string? ErrorMessage { get; set; }

    public ValidationRuleAttribute(string ruleType)
    {
        RuleType = ruleType;
    }
}
