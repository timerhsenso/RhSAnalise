// =============================================================================
// EXEMPLO DE USO - ENTITY COM [GenerateCrud]
// =============================================================================
// Este arquivo demonstra como marcar uma Entity para geração automática de CRUD.
// Caminho: src/Identity/Domain/Entities/Sistema.cs
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Generators.Attributes;

namespace RhSensoERP.Identity.Domain.Entities;

/// <summary>
/// Entidade Sistema - Representa um módulo/sistema do ERP.
/// Esta é uma TABELA LEGADA que não herda de BaseEntity.
/// </summary>
[GenerateCrud(
    // =========================================================================
    // CONFIGURAÇÕES BÁSICAS
    // =========================================================================
    TableName = "tsistema",           // Nome da tabela no banco SQL Server
    Schema = "dbo",                   // Schema (padrão: dbo)
    DisplayName = "Sistema",          // Nome amigável para logs e mensagens

    // =========================================================================
    // CONFIGURAÇÕES DE MÓDULO E PERMISSÕES
    // =========================================================================
    CdSistema = "SEG",                // Código do módulo (SEG = Segurança)
    CdFuncao = "SEG_FM_TSISTEMA",     // Código da tela para verificação de permissões IAEC

    // =========================================================================
    // CONFIGURAÇÕES DE ROTA (OPCIONAL - valores padrão serão inferidos)
    // =========================================================================
    // ApiRoute = "identity/sistemas",    // Se omitido: inferido do namespace
    // ApiGroup = "Identity",             // Se omitido: inferido do CdSistema

    // =========================================================================
    // TABELA LEGADA
    // =========================================================================
    IsLegacyTable = true              // Indica que NÃO herda de BaseEntity

    // =========================================================================
    // FLAGS DE GERAÇÃO BACKEND (todos true por padrão)
    // =========================================================================
    // GenerateDto = true,
    // GenerateRequests = true,
    // GenerateCommands = true,
    // GenerateQueries = true,
    // GenerateValidators = true,
    // GenerateRepository = true,
    // GenerateMapper = true,
    // GenerateEfConfig = true,
    // SupportsBatchDelete = true,

    // =========================================================================
    // FLAGS DE GERAÇÃO WEB/API (todos FALSE por padrão!)
    // Esses arquivos devem ser copiados manualmente para os projetos Web/API
    // =========================================================================
    // GenerateApiController = false,
    // GenerateWebController = false,
    // GenerateWebModels = false,
    // GenerateWebServices = false
)]
public class Sistema  // ← SEM HERANÇA DE BaseEntity (IsLegacyTable = true)
{
    /// <summary>
    /// Código do sistema (PK).
    /// Exemplo: "SEG", "RHU", "FIN"
    /// </summary>
    [Key]
    [Column("cdsistema")]             // ← Use [Column] do EF Core, NÃO [ColumnName]!
    [StringLength(10)]
    [FieldDisplayName("Código")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema.
    /// Exemplo: "Segurança", "Recursos Humanos", "Financeiro"
    /// </summary>
    [Required]
    [Column("dcsistema")]             // ← Use [Column] do EF Core
    [StringLength(100)]
    [FieldDisplayName("Descrição")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o sistema está ativo.
    /// "S" = Ativo, "N" = Inativo
    /// </summary>
    [Column("ativo")]                 // ← Use [Column] do EF Core
    [StringLength(1)]
    [FieldDisplayName("Ativo")]
    public string Ativo { get; set; } = "S";
}

// =============================================================================
// EXEMPLO 2: ENTITY NOVA (COM BaseEntity)
// =============================================================================
// Para tabelas NOVAS que herdam de BaseEntity:

/*
[GenerateCrud(
    TableName = "produtos",
    DisplayName = "Produto",
    CdSistema = "EST",
    CdFuncao = "EST_FM_TPRODUTO"
    // IsLegacyTable = false (padrão)
)]
public class Produto : BaseEntity  // ← HERDA DE BaseEntity
{
    [Required]
    [StringLength(100)]
    [FieldDisplayName("Nome")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    [FieldDisplayName("Descrição")]
    public string? Descricao { get; set; }

    [Required]
    [FieldDisplayName("Preço")]
    public decimal Preco { get; set; }

    [FieldDisplayName("Ativo")]
    public bool Ativo { get; set; } = true;
}
*/
