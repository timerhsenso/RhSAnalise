// Mfre2.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Parametrização local de motivos de ocorrência (por empresa/filial).
/// </summary>
[Table("mfre2")]
public class Mfre2 : BaseEntity
{
    /// <summary>Chave técnica (tabela não define PK explícita).</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdmotoc"), StringLength(4)] public string CdMotOc { get; set; } = default!;
    [Column("tpocorr")] public int TpOcorr { get; set; }
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("idfilial")] public Guid? IdFilial { get; set; }
    [Column("idmotivosdeocorrenciafrequencia")] public Guid? IdMotivosDeOcorrenciaFrequencia { get; set; }
}
