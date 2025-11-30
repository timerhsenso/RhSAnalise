using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

[GenerateCrud(
    TableName = "taux2",
    DisplayName = "Taux2",
    CdSistema = "SEG",
    CdFuncao = "SEG_FM_TSISTEMA",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class Taux2
{
    #region Propriedades

    [Key]
    [Required]
    [Column("cdtptabela")]
    [StringLength(2)]
    [FieldDisplayName("Cdtptabela")]
    public string Cdtptabela { get; set; } = string.Empty;

    [Required]
    [Column("cdsituacao")]
    [StringLength(2)]
    [FieldDisplayName("Cdsituacao")]
    public string Cdsituacao { get; set; } = string.Empty;

    [Required]
    [Column("dcsituacao")]
    [StringLength(60)]
    [FieldDisplayName("Dcsituacao")]
    public string Dcsituacao { get; set; } = string.Empty;

    [Column("noordem")]
    [FieldDisplayName("Noordem")]
    public int? Noordem { get; set; }

    [Column("flativoaux")]
    [StringLength(1)]
    [FieldDisplayName("Flativoaux")]
    public string Flativoaux { get; set; } = string.Empty;

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Navegação para Taux1 via cdtptabela
    /// </summary>
    [ForeignKey(nameof(Cdtptabela))]
    public virtual Tabtaux1 cdtptabela { get; set; }

    #endregion
}
