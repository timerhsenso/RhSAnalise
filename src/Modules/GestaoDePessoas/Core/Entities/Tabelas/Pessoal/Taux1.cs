using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

[GenerateCrud(
    TableName = "taux1",
    DisplayName = "Tabela Auxiliar",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TAUX1",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class Taux1
{
    [Key]
    [Required]
    [StringLength(2)]
    public string CdTptabela { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Dctabela { get; set; } = string.Empty;

}
