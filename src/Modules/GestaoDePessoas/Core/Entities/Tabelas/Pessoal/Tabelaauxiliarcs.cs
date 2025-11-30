using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

[GenerateCrud(
    TableName = "taux1",
    DisplayName = "Tipo de Tabela",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TAUX1",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class Tabtaux1
{
    [Key]
    [Required]
    [Column("cdtptabela")]
    [StringLength(2)]
    [FieldDisplayName("Código do Tipo de Tabela")]
    public string CdTpTabela { get; set; } = string.Empty;

    [Required]
    [Column("dctabela")]
    [StringLength(60)]
    [FieldDisplayName("Descrição da Tabela")]
    public string DcTabela { get; set; } = string.Empty;
}
