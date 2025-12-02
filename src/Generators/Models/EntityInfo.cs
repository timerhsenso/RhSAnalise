// =============================================================================
// RHSENSOERP GENERATOR v3.2 - ENTITY INFO MODEL
// =============================================================================
// Arquivo: src/Generators/Models/EntityInfo.cs
// Versão: 3.2 - CORREÇÃO: PKs de texto (não auto-geradas) aparecem no Create
// =============================================================================

namespace RhSensoERP.Generators.Models;

/// <summary>
/// Modelo que contém todas as informações extraídas de uma Entity
/// para geração de código.
/// </summary>
public class EntityInfo
{
    // =========================================================================
    // IDENTIFICAÇÃO DA ENTITY
    // =========================================================================

    /// <summary>
    /// Nome da classe da Entity (ex: "Sistema", "Usuario").
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Nome completo com namespace (ex: "RhSensoERP.Identity.Domain.Entities.Sistema").
    /// </summary>
    public string FullClassName { get; set; } = string.Empty;

    /// <summary>
    /// Namespace da Entity (ex: "RhSensoERP.Identity.Domain.Entities").
    /// </summary>
    public string Namespace { get; set; } = string.Empty;

    /// <summary>
    /// Nome amigável para exibição (ex: "Sistema", "Usuário").
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Nome no plural (ex: "Sistemas", "Usuarios").
    /// </summary>
    public string PluralName { get; set; } = string.Empty;

    // =========================================================================
    // MÓDULO E PERMISSÕES
    // =========================================================================

    /// <summary>
    /// Módulo inferido do namespace (ex: "Identity", "GestaoDePessoas", "Shared").
    /// </summary>
    public string ModuleName { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o módulo está na pasta Modules (true) ou na raiz (false).
    /// Ex: RhSensoERP.Modules.GestaoDePessoas = true
    /// Ex: RhSensoERP.Identity = false
    /// </summary>
    public bool IsModulesStructure { get; set; }

    /// <summary>
    /// Código do sistema para permissões (ex: "SEG", "RHU").
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Código da função para verificação de permissões (ex: "SEG_FM_TSISTEMA").
    /// </summary>
    public string CdFuncao { get; set; } = string.Empty;

    // =========================================================================
    // CONFIGURAÇÃO DA TABELA
    // =========================================================================

    /// <summary>
    /// Nome da tabela no banco de dados.
    /// </summary>
    public string TableName { get; set; } = string.Empty;

    /// <summary>
    /// Schema da tabela (padrão: "dbo").
    /// </summary>
    public string Schema { get; set; } = "dbo";

    /// <summary>
    /// Indica se é tabela legada (sem BaseEntity).
    /// </summary>
    public bool IsLegacyTable { get; set; }

    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    /// <summary>
    /// Nome da propriedade que é a PK (ex: "CdSistema", "Id").
    /// </summary>
    public string PrimaryKeyProperty { get; set; } = "Id";

    /// <summary>
    /// Nome da coluna da PK no banco (ex: "cdsistema", "id").
    /// </summary>
    public string PrimaryKeyColumn { get; set; } = "id";

    /// <summary>
    /// Tipo C# da PK (ex: "string", "int", "Guid").
    /// </summary>
    public string PrimaryKeyType { get; set; } = "Guid";

    /// <summary>
    /// Indica se a PK é gerada pelo banco (Identity/AutoIncrement).
    /// </summary>
    public bool PrimaryKeyIsGenerated { get; set; }

    // =========================================================================
    // ROTAS E API
    // =========================================================================

    /// <summary>
    /// Rota base da API (ex: "identity/sistemas").
    /// </summary>
    public string ApiRoute { get; set; } = string.Empty;

    /// <summary>
    /// Rota completa da API (ex: "/api/identity/sistemas").
    /// </summary>
    public string ApiFullRoute => $"/api/{ApiRoute}";

    /// <summary>
    /// Nome do grupo no Swagger (ex: "Identity", "RHU").
    /// </summary>
    public string ApiGroup { get; set; } = string.Empty;

    // =========================================================================
    // FLAGS DE GERAÇÃO - BACKEND
    // =========================================================================

    public bool GenerateDto { get; set; } = true;
    public bool GenerateRequests { get; set; } = true;
    public bool GenerateCommands { get; set; } = true;
    public bool GenerateQueries { get; set; } = true;
    public bool GenerateValidators { get; set; } = true;
    public bool GenerateRepository { get; set; } = true;
    public bool GenerateMapper { get; set; } = true;
    public bool GenerateEfConfig { get; set; } = true;

    /// <summary>
    /// Gera o MetadataProvider para UI dinâmica (NOVO v3.1).
    /// </summary>
    public bool GenerateMetadata { get; set; } = true;

    // =========================================================================
    // FLAGS DE GERAÇÃO - API/WEB
    // =========================================================================

    /// <summary>
    /// Gera o API Controller.
    /// </summary>
    public bool GenerateApiController { get; set; } = true;

    public bool GenerateWebController { get; set; } = false;
    public bool GenerateWebModels { get; set; } = false;
    public bool GenerateWebServices { get; set; } = false;
    public bool SupportsBatchDelete { get; set; } = true;
    public bool ApiRequiresAuth { get; set; } = true;

    // =========================================================================
    // PROPRIEDADES DA ENTITY
    // =========================================================================

    /// <summary>
    /// Lista de todas as propriedades da Entity.
    /// </summary>
    public List<PropertyInfo> Properties { get; set; } = new();

    /// <summary>
    /// Propriedades que vão para o DTO de leitura.
    /// </summary>
    public IEnumerable<PropertyInfo> DtoProperties =>
        Properties.Where(p => !p.ExcludeFromDto);

    /// <summary>
    /// Propriedades editáveis no Create.
    /// CORREÇÃO v3.2: PKs não auto-geradas (texto/código manual) são incluídas!
    /// </summary>
    public IEnumerable<PropertyInfo> CreateProperties =>
        Properties.Where(p => !p.IsReadOnly && !p.IsAutoGeneratedKey);

    /// <summary>
    /// Propriedades editáveis no Update.
    /// PKs nunca são editáveis no Update (nem mesmo as de texto).
    /// </summary>
    public IEnumerable<PropertyInfo> UpdateProperties =>
        Properties.Where(p => !p.IsReadOnly && !p.IsPrimaryKey);

    /// <summary>
    /// Propriedades obrigatórias na criação.
    /// CORREÇÃO v3.2: PKs de texto são obrigatórias na criação.
    /// </summary>
    public IEnumerable<PropertyInfo> RequiredProperties =>
        Properties.Where(p => p.IsRequired || p.RequiredOnCreate || (p.IsPrimaryKey && !p.IsAutoGeneratedKey));

    // =========================================================================
    // NAMESPACES GERADOS
    // =========================================================================

    /// <summary>
    /// Namespace base do módulo (ex: "RhSensoERP.Identity" ou "RhSensoERP.Modules.GestaoDePessoas").
    /// </summary>
    public string ModuleNamespace { get; set; } = string.Empty;

    /// <summary>
    /// Namespace dos DTOs.
    /// </summary>
    public string DtoNamespace => $"{ModuleNamespace}.Application.DTOs.{PluralName}";

    /// <summary>
    /// Namespace dos Commands.
    /// </summary>
    public string CommandsNamespace => $"{ModuleNamespace}.Application.Features.{PluralName}.Commands";

    /// <summary>
    /// Namespace das Queries.
    /// </summary>
    public string QueriesNamespace => $"{ModuleNamespace}.Application.Features.{PluralName}.Queries";

    /// <summary>
    /// Namespace dos Validators.
    /// </summary>
    public string ValidatorsNamespace => $"{ModuleNamespace}.Application.Validators.{PluralName}";

    /// <summary>
    /// Namespace do Repository Interface.
    /// </summary>
    public string RepositoryInterfaceNamespace => $"{ModuleNamespace}.Core.Interfaces.Repositories";

    /// <summary>
    /// Namespace do Repository Implementation.
    /// </summary>
    public string RepositoryImplNamespace => $"{ModuleNamespace}.Infrastructure.Repositories";

    /// <summary>
    /// Namespace do Mapper.
    /// </summary>
    public string MapperNamespace => $"{ModuleNamespace}.Application.Mapping";

    /// <summary>
    /// Namespace do EF Config.
    /// </summary>
    public string EfConfigNamespace => $"{ModuleNamespace}.Infrastructure.Persistence.Configurations";

    /// <summary>
    /// Namespace do MetadataProvider (NOVO v3.1).
    /// </summary>
    public string MetadataNamespace => $"{ModuleNamespace}.Application.Metadata";

    /// <summary>
    /// Namespace do API Controller.
    /// </summary>
    public string ApiControllerNamespace => $"RhSensoERP.API.Controllers.{ModuleName}";

    /// <summary>
    /// Namespace do Web Controller.
    /// </summary>
    public string WebControllerNamespace => "RhSensoERP.Web.Controllers";

    /// <summary>
    /// Namespace dos Web Models.
    /// </summary>
    public string WebModelsNamespace => $"RhSensoERP.Web.Models.{PluralName}";

    /// <summary>
    /// Namespace dos Web Services.
    /// </summary>
    public string WebServicesNamespace => $"RhSensoERP.Web.Services.{PluralName}";

    // =========================================================================
    // DBCONTEXT - PROPRIEDADES CALCULADAS
    // =========================================================================

    /// <summary>
    /// Namespace do DbContext do módulo.
    /// Para Modules: RhSensoERP.Modules.{Nome}.Infrastructure.Persistence.Contexts
    /// Para Identity: RhSensoERP.Identity.Infrastructure.Persistence
    /// </summary>
    public string DbContextNamespace => IsModulesStructure
        ? $"{ModuleNamespace}.Infrastructure.Persistence.Contexts"
        : $"{ModuleNamespace}.Infrastructure.Persistence";

    /// <summary>
    /// Nome do DbContext do módulo (ex: "GestaoDePessoasDbContext", "IdentityDbContext").
    /// </summary>
    public string DbContextName => $"{ModuleName}DbContext";
}

/// <summary>
/// Informações de uma propriedade da Entity.
/// </summary>
public class PropertyInfo
{
    /// <summary>
    /// Nome da propriedade C# (ex: "CdSistema", "DcSistema").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tipo C# da propriedade (ex: "string", "int?", "DateTime").
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Nome da coluna no banco (se diferente do nome da propriedade).
    /// </summary>
    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Nome amigável para exibição.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Indica se é a chave primária.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    /// Indica se é obrigatório ([Required]).
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Indica se é obrigatório apenas na criação.
    /// </summary>
    public bool RequiredOnCreate { get; set; }

    /// <summary>
    /// Indica se é somente leitura (não editável).
    /// </summary>
    public bool IsReadOnly { get; set; }

    /// <summary>
    /// Indica se deve ser excluído do DTO de leitura.
    /// </summary>
    public bool ExcludeFromDto { get; set; }

    /// <summary>
    /// Indica se é nullable (tipo termina com ?).
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Comprimento máximo da string ([StringLength] ou [MaxLength]).
    /// </summary>
    public int? MaxLength { get; set; }

    /// <summary>
    /// Comprimento mínimo da string ([MinLength]).
    /// </summary>
    public int? MinLength { get; set; }

    /// <summary>
    /// Valor padrão da propriedade.
    /// </summary>
    public string? DefaultValue { get; set; }

    // =========================================================================
    // NOVO v3.2: Propriedades para controle de PKs auto-geradas
    // =========================================================================

    /// <summary>
    /// Indica se é Identity (auto-incremento no banco).
    /// [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    /// </summary>
    public bool IsIdentity { get; set; }

    /// <summary>
    /// Indica se é uma chave auto-gerada (Identity ou Guid).
    /// PKs de texto (código manual) NÃO são auto-geradas.
    /// </summary>
    public bool IsAutoGeneratedKey => IsPrimaryKey && (IsIdentity || IsGuid);

    /// <summary>
    /// Indica se é uma PK de texto (código manual, editável na criação).
    /// </summary>
    public bool IsManualPrimaryKey => IsPrimaryKey && !IsAutoGeneratedKey;

    // =========================================================================
    // Propriedades calculadas de tipo
    // =========================================================================

    /// <summary>
    /// Tipo base sem nullable (ex: "string" para "string?").
    /// </summary>
    public string BaseType => Type.TrimEnd('?');

    /// <summary>
    /// Verifica se é tipo string.
    /// </summary>
    public bool IsString => BaseType.Equals("string", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifica se é tipo numérico.
    /// </summary>
    public bool IsNumeric => BaseType is "int" or "long" or "decimal" or "double" or "float" or "short" or "byte";

    /// <summary>
    /// Verifica se é tipo DateTime.
    /// </summary>
    public bool IsDateTime => BaseType is "DateTime" or "DateTimeOffset" or "DateOnly" or "TimeOnly";

    /// <summary>
    /// Verifica se é tipo bool.
    /// </summary>
    public bool IsBool => BaseType is "bool" or "Boolean";

    /// <summary>
    /// Verifica se é tipo Guid.
    /// </summary>
    public bool IsGuid => BaseType is "Guid";
}