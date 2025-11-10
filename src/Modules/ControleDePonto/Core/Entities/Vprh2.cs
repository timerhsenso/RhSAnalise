// Vprh2.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Valores efetivos por data para cada parâmetro de RH.
/// </summary>
[Table("vprh2")]
public class Vprh2 : BaseEntity
{
    /// <summary>Chave técnica (tabela não define PK explícita).</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdvalor"), StringLength(4)] public string CdValor { get; set; } = default!;
    [Column("dtvalor")] public DateTime DtValor { get; set; }
    [Column("vlparam")] public double VlParam { get; set; } // float -> double
    [Column("idvalor")] public Guid? IdValor { get; set; }
}
