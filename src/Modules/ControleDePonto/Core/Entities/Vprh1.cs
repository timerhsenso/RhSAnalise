using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Tabela-mestre de valores parametrizados de RH.
/// </summary>
[Table("vprh1")]
public class Vprh1 : BaseEntity
{
    /// <summary>Chave técnica (tabela não define PK explícita).</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdvalor"), StringLength(4)] public string CdValor { get; set; } = default!;
    [Column("dcvalor"), StringLength(100)] public string DcValor { get; set; } = default!;
}
