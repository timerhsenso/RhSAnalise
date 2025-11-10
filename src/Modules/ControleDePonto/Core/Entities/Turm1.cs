using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Cadastro de turmas/turnos.
/// </summary>
[Table("turm1")]
public class Turm1 : BaseEntity
{
    /// <summary>Chave técnica (tabela não define PK explícita).</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("cdturma"), StringLength(2)] public string CdTurma { get; set; } = default!;
    [Column("dcturma"), StringLength(20)] public string? DcTurma { get; set; }
    [Column("MMTOLERANCIA")] public int? MmTolerancia { get; set; }
    [Column("MMTOLERANCIA2")] public int? MmTolerancia2 { get; set; }
    [Column("idfilial")] public Guid? IdFilial { get; set; }
}
