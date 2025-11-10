// Chor1.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Grade de horário administrativo (faixas fixas).
/// </summary>
[Table("chor1")]
public class Chor1 : BaseEntity
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdcarghor"), StringLength(2)] public string CdCargHor { get; set; } = default!;
    [Column("hhentrada"), StringLength(5)] public string HhEntrada { get; set; } = default!;
    [Column("hhsaida"), StringLength(5)] public string HhSaida { get; set; } = default!;
    [Column("hhinicint"), StringLength(5)] public string HhIniInt { get; set; } = default!;
    [Column("hhfimint"), StringLength(5)] public string HhFimInt { get; set; } = default!;
    [Column("MMTOLERANCIA")] public int? MmTolerancia { get; set; }
    [Column("FLINTERVALO"), StringLength(1)] public string? FlIntervalo { get; set; }
    [Column("MMTOLERANCIA2")] public int? MmTolerancia2 { get; set; }
    [Column("DCCARGHOR"), StringLength(100)] public string DcCargHor { get; set; } = default!;
    [Column("codhors1050"), StringLength(30)] public string? CodHors1050 { get; set; }
}
