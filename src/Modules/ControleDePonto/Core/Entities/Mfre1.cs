// Mfre1.cs
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Motivos/Tipos de ocorrência de frequência (matriz).
/// </summary>
[Table("mfre1")]
[PrimaryKey(nameof(TpOcorr), nameof(CdMotOc))]
public class Mfre1 : BaseEntity
{
    [Column("cdmotoc"), StringLength(4)] public string CdMotOc { get; set; } = default!;
    [Column("tpocorr")] public int TpOcorr { get; set; }

    [Column("dcmotoc"), StringLength(40)] public string? DcMotOc { get; set; }
    [Column("flmovimen")] public int? FlMovimen { get; set; }
    [Column("cdconta"), StringLength(4)] public string? CdConta { get; set; }
    [Column("fltpfal")] public int? FlTpFal { get; set; }
    [Column("flextra")] public int? FlExtra { get; set; }
    [Column("flflanj")] public int? FlFlAnj { get; set; }
    [Column("FLTROCA")] public int? FlTroca { get; set; }
    [Column("FLREGRAHE")] public int? FlRegraHe { get; set; }
    [Column("FLBANCOHORAS")] public int FlBancoHoras { get; set; }
    [Column("TPOCORRLINK")] public int? TpOcorrLink { get; set; }
    [Column("CDMOTOCLINK"), StringLength(4)] public string? CdMotOcLink { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("idmotivosdeocorrenciafrequenciapai")] public Guid? IdMotivoPai { get; set; }
    [Column("idverba")] public Guid? IdVerba { get; set; }
}
