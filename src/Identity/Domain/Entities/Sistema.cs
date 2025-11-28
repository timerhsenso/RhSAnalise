// src/Identity/Domain/Entities/Sistema.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Identity.Domain.Entities;

[GenerateCrud(
    TableName = "tsistema",
    DisplayName = "Sistema",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = true,
    GenerateApiController = true  // ← ADICIONAR ISSO!

)]
public class Sistema
{
    [Key]
    [Column("cdsistema")]
    [StringLength(10)]
    [FieldDisplayName("Código")]
    public string CdSistema { get; set; } = string.Empty;

    [Required]
    [Column("dcsistema")]
    [StringLength(100)]
    [FieldDisplayName("Descrição")]
    public string DcSistema { get; set; } = string.Empty;

    [Column("ativo")]
    [FieldDisplayName("Ativo")]
    public bool Ativo { get; set; } = true;

    // ═══════════════════════════════════════════════════════════════
    // NAVEGAÇÃO - Relacionamento 1:N com Funcao
    // ═══════════════════════════════════════════════════════════════
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}