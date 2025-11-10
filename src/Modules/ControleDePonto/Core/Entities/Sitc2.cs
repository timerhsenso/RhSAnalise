using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Situação de fechamento/processamento de frequência por colaborador/dia.
/// </summary>
[Table("sitc2")]
public class Sitc2 : BaseEntity
{
    /// <summary>Chave técnica (tabela não define PK explícita).</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("nomatric"), StringLength(8)] public string NoMatric { get; set; } = default!;
    [Column("dtfrequen")] public DateTime DtFrequen { get; set; }
    [Column("flsituacao")] public int FlSituacao { get; set; }
    [Column("cdusuario"), StringLength(20)] public string? CdUsuario { get; set; }
    [Column("dtultmov")] public DateTime DtUltMov { get; set; }
    [Column("FLPROCESSADO")] public int FlProcessado { get; set; }
    [Column("FLIMPORTADO")] public int FlImportado { get; set; }
    [Column("DTIMPORTACAO")] public DateTime? DtImportacao { get; set; }
    [Column("DTPROCESSAMENTO")] public DateTime? DtProcessamento { get; set; }
    [Column("idfuncionario")] public Guid? IdFuncionario { get; set; }
}
