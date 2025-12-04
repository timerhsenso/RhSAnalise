using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Tabela 10 - Tipos de Lotação Tributária (eSocial)
/// Evento S-1020
/// Tabela: tab10_esocial
/// </summary>
[GenerateCrud(
    TableName = "tab10_esocial",
    DisplayName = "Tabela 10 - Tipo de Lotação Tributária",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_TAB10",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tab10_esocial")]
public class EsocialTabela10
{
    [Key]
    [Column("tab10_codigo")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Tab10Codigo { get; set; } = string.Empty;

    [Column("tab10_descricao")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Tab10Descricao { get; set; } = string.Empty;

    [Column("tab10_desc_doc_requisito")]
    [StringLength(255)]
    [Display(Name = "Documentos/Requisitos")]
    public string? Tab10DescDocRequisito { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Collections - DENTRO do mesmo módulo (Esocial) ✅
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lotações tributárias que utilizam este tipo
    /// </summary>
    [InverseProperty(nameof(LotacaoTributaria.Tabela10))]
    public virtual ICollection<LotacaoTributaria> Lotacoes { get; set; } = new List<LotacaoTributaria>();
}
