using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Tabela 4 - Códigos e Alíquotas de FPAS/Terceiros (eSocial)
/// Fundo de Previdência e Assistência Social
/// Tabela: tab4_esocial
/// </summary>
[GenerateCrud(
    TableName = "tab4_esocial",
    DisplayName = "Tabela 4 - FPAS",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_TAB04",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tab4_esocial")]
public class EsocialTabela4
{
    [Key]
    [Column("tab4_codigo")]
    [StringLength(3)]
    [Display(Name = "Código FPAS")]
    public string Tab4Codigo { get; set; } = string.Empty;

    [Column("tab4_descricao")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Tab4Descricao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // Collections - DENTRO do mesmo módulo (Esocial) ✅
    // ═══════════════════════════════════════════════════════════════════

    /// <summary>
    /// Lotações tributárias que utilizam este FPAS
    /// </summary>
    [InverseProperty(nameof(LotacaoTributaria.Tabela4))]
    public virtual ICollection<LotacaoTributaria> Lotacoes { get; set; } = new List<LotacaoTributaria>();
}
