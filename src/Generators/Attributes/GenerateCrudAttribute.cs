// =============================================================================
// RHSENSOERP GENERATOR v3.0 - GENERATE CRUD ATTRIBUTE
// =============================================================================
// Arquivo: src/Generators/Attributes/GenerateCrudAttribute.cs
// Versão: 3.1 - Com suporte a Metadata-Driven UI
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
    // CONFIGURAÇÕES DE MÓDULO E PERMISSÕES
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

    /// <summary>
    /// Gera o MetadataProvider para UI dinâmica (NOVO v3.1).
    /// </summary>
    public bool GenerateMetadata { get; set; } = true;

    // =========================================================================
    // FLAGS DE GERAÇÃO - API CONTROLLER
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
    // FLAGS DE GERAÇÃO - WEB
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

    // =========================================================================
    // CONFIGURAÇÕES DE UI (NOVO v3.1)
    // =========================================================================

    /// <summary>
    /// Ícone da entidade (FontAwesome ou outra lib).
    /// Exemplo: "fa-users", "fa-building"
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Tamanho padrão da página no DataTable.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Campo padrão para ordenação.
    /// </summary>
    public string DefaultSortField { get; set; } = string.Empty;

    /// <summary>
    /// Direção padrão de ordenação ("asc" ou "desc").
    /// </summary>
    public string DefaultSortDirection { get; set; } = "asc";

    /// <summary>
    /// Permite exportação para Excel.
    /// </summary>
    public bool CanExport { get; set; } = true;

    /// <summary>
    /// Permite importação de Excel.
    /// </summary>
    public bool CanImport { get; set; } = false;

    /// <summary>
    /// Permite impressão.
    /// </summary>
    public bool CanPrint { get; set; } = true;
}

// =============================================================================
// ATRIBUTOS DE CAMPO - BÁSICOS
// =============================================================================

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

// =============================================================================
// ATRIBUTOS DE UI - LISTAGEM (DataTable) - NOVO v3.1
// =============================================================================

/// <summary>
/// Configura o comportamento do campo na listagem (DataTable).
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class ListConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir na listagem. Padrão: true
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Ordem de exibição na listagem. Menor = mais à esquerda.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Largura da coluna em pixels. 0 = automático.
    /// </summary>
    public int Width { get; set; } = 0;

    /// <summary>
    /// Permite ordenação por esta coluna.
    /// </summary>
    public bool Sortable { get; set; } = true;

    /// <summary>
    /// Permite filtro por esta coluna.
    /// </summary>
    public bool Filterable { get; set; } = true;

    /// <summary>
    /// Formato de exibição: "date", "datetime", "currency", "percent", "boolean"
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Classe CSS adicional para a coluna.
    /// </summary>
    public string CssClass { get; set; } = string.Empty;

    /// <summary>
    /// Alinhamento: "left", "center", "right"
    /// </summary>
    public string Align { get; set; } = "left";
}

// =============================================================================
// ATRIBUTOS DE UI - FORMULÁRIO - NOVO v3.1
// =============================================================================

/// <summary>
/// Configura o comportamento do campo no formulário.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FormConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir no formulário. Padrão: true
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de criação.
    /// </summary>
    public bool ShowOnCreate { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de edição.
    /// </summary>
    public bool ShowOnEdit { get; set; } = true;

    /// <summary>
    /// Ordem de exibição no formulário.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Grupo/seção do formulário para agrupar campos relacionados.
    /// Exemplo: "Dados Pessoais", "Endereço", "Contato"
    /// </summary>
    public string Group { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de input HTML: "text", "textarea", "select", "date", "datetime", 
    /// "time", "number", "email", "tel", "url", "password", "checkbox", "radio", "hidden"
    /// </summary>
    public string InputType { get; set; } = "text";

    /// <summary>
    /// Texto placeholder do campo.
    /// </summary>
    public string Placeholder { get; set; } = string.Empty;

    /// <summary>
    /// Texto de ajuda exibido abaixo do campo.
    /// </summary>
    public string HelpText { get; set; } = string.Empty;

    /// <summary>
    /// Máscara de input (para campos formatados).
    /// Exemplo: "000.000.000-00" (CPF), "(00) 00000-0000" (telefone)
    /// </summary>
    public string Mask { get; set; } = string.Empty;

    /// <summary>
    /// Número de linhas para textarea.
    /// </summary>
    public int Rows { get; set; } = 3;

    /// <summary>
    /// Número de colunas do grid (1-12, Bootstrap).
    /// </summary>
    public int ColSize { get; set; } = 6;

    /// <summary>
    /// Ícone do campo (FontAwesome).
    /// </summary>
    public string Icon { get; set; } = string.Empty;
}

// =============================================================================
// ATRIBUTOS DE UI - LOOKUP/RELACIONAMENTO - NOVO v3.1
// =============================================================================

/// <summary>
/// Configura um campo como lookup (dropdown/autocomplete) para relacionamentos.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class LookupAttribute : Attribute
{
    /// <summary>
    /// Endpoint da API para buscar as opções.
    /// Exemplo: "/api/rhu/bancos"
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Nome do campo que contém o valor (ID).
    /// Padrão: "Id"
    /// </summary>
    public string ValueField { get; set; } = "Id";

    /// <summary>
    /// Nome do campo que contém o texto de exibição.
    /// Padrão: "Nome"
    /// </summary>
    public string TextField { get; set; } = "Nome";

    /// <summary>
    /// Permite busca/autocomplete.
    /// </summary>
    public bool AllowSearch { get; set; } = true;

    /// <summary>
    /// Permite limpar a seleção.
    /// </summary>
    public bool AllowClear { get; set; } = true;

    /// <summary>
    /// Campo pai para lookup em cascata.
    /// Exemplo: Se Estado depende de País, DependsOn = "PaisId"
    /// </summary>
    public string DependsOn { get; set; } = string.Empty;

    /// <summary>
    /// Parâmetro de filtro enviado ao endpoint pai.
    /// Exemplo: "paisId" para filtrar estados por país.
    /// </summary>
    public string DependsOnParam { get; set; } = string.Empty;

    /// <summary>
    /// Número mínimo de caracteres para iniciar busca.
    /// </summary>
    public int MinSearchLength { get; set; } = 0;

    /// <summary>
    /// Permite seleção múltipla.
    /// </summary>
    public bool Multiple { get; set; } = false;
}

// =============================================================================
// ATRIBUTOS DE UI - AÇÕES CUSTOMIZADAS - NOVO v3.1
// =============================================================================

/// <summary>
/// Define uma ação customizada para a entidade.
/// Exemplo: "Aprovar", "Rejeitar", "Enviar Email"
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class EntityActionAttribute : Attribute
{
    /// <summary>
    /// Identificador único da ação.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Texto exibido no botão.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Ícone do botão (FontAwesome).
    /// </summary>
    public string Icon { get; set; } = string.Empty;

    /// <summary>
    /// Classe CSS do botão.
    /// Exemplo: "btn-success", "btn-warning", "btn-danger"
    /// </summary>
    public string CssClass { get; set; } = "btn-secondary";

    /// <summary>
    /// Mensagem de confirmação. Se vazio, não pede confirmação.
    /// </summary>
    public string ConfirmMessage { get; set; } = string.Empty;

    /// <summary>
    /// Endpoint da API para executar a ação.
    /// Exemplo: "/api/rhu/funcionarios/{id}/aprovar"
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Método HTTP: "POST", "PUT", "PATCH", "DELETE"
    /// </summary>
    public string HttpMethod { get; set; } = "POST";

    /// <summary>
    /// Exibir na listagem (por registro).
    /// </summary>
    public bool ShowInList { get; set; } = true;

    /// <summary>
    /// Exibir no formulário de detalhes/edição.
    /// </summary>
    public bool ShowInForm { get; set; } = true;

    /// <summary>
    /// Código de permissão necessária para executar a ação.
    /// </summary>
    public string RequiredPermission { get; set; } = string.Empty;

    public EntityActionAttribute(string name)
    {
        Name = name;
    }
}

// =============================================================================
// ATRIBUTOS DE UI - FILTROS - NOVO v3.1
// =============================================================================

/// <summary>
/// Configura um filtro avançado para a listagem.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
public sealed class FilterConfigAttribute : Attribute
{
    /// <summary>
    /// Exibir no painel de filtros avançados.
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// Tipo de filtro: "text", "select", "date", "daterange", "number", "numberrange", "boolean"
    /// </summary>
    public string FilterType { get; set; } = "text";

    /// <summary>
    /// Operador padrão: "equals", "contains", "startswith", "endswith", "gt", "gte", "lt", "lte", "between"
    /// </summary>
    public string DefaultOperator { get; set; } = "contains";

    /// <summary>
    /// Ordem no painel de filtros.
    /// </summary>
    public int Order { get; set; } = 0;

    /// <summary>
    /// Placeholder do campo de filtro.
    /// </summary>
    public string Placeholder { get; set; } = string.Empty;
}