using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Lançamentos de Banco de Horas por funcionário/dia.
/// </summary>
/// 
[Table("BancoHoras")]
public class BancoHoras : BaseEntity
{
    /// <summary>Identificador interno (PK).</summary>
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CDEMPRESA")] public int CdEmpresa { get; set; }

    [Column("CDFILIAL")] public int CdFilial { get; set; }

    [Column("NOMATRIC"), StringLength(8)]

    public string NoMatric { get; set; } = default!;
    
    [Column("DATA")] public DateTime Data { get; set; }

    [Column("ORDEM")] public int Ordem { get; set; }

    [Column("TEMPO")] public int Tempo { get; set; }

    [Column("DEBCRED"), StringLength(1)]
    public string DebCred { get; set; } = default!;

    [Column("TIPO"), StringLength(1)]
    public string Tipo { get; set; } = default!;

    [Column("DESCRICAO"), StringLength(100)]

    public string? Descricao { get; set; }

    [Column("CDCONTA"), StringLength(4)]

    public string? CdConta { get; set; }

    [Column("SALDOANTERIOR")]

    public int SaldoAnterior { get; set; }

    [Column("DATA_FREQ1")] public DateTime? DataFreq1 { get; set; }

    [Column("INICIO_FREQ1")] public DateTime? InicioFreq1 { get; set; }
}
