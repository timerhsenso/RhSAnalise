using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

[GenerateCrud(
    TableName = "tcbo1",
    DisplayName = "Tabela de Ocupação",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TCBO1",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class Tcbo1
{
    [Key]
    [Required]
    [StringLength(6)]
    public string Cdcbo { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Dccbo { get; set; } = string.Empty;

    [StringLength(4000)]
    public string SiNonimo { get; set; } = string.Empty;

    public Guid Id { get; set; }

}
