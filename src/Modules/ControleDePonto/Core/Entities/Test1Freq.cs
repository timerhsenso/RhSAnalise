using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Parâmetros de cálculo/ajustes globais de frequência por empresa/filial.
/// </summary>
[Table("test1_freq")]
[PrimaryKey(nameof(CdEmpresa), nameof(CdFilial))]
public class Test1Freq : BaseEntity
{
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }

    [Column("DivideExta"), StringLength(1)] public string? DivideExta { get; set; }
    [Column("CodOcor1"), StringLength(4)] public string? CodOcor1 { get; set; }
    [Column("CodOcor2"), StringLength(4)] public string? CodOcor2 { get; set; }
    [Column("QtdHoraExtraPadrao")] public int? QtdHoraExtraPadrao { get; set; }
}
