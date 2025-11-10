using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Períodos de frequência (início/fim) por funcionário.
/// </summary>
[Table("freq2")]
public class Freq2 : BaseEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("nomatric"), StringLength(8)] public string NoMatric { get; set; } = default!;
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("data")] public DateTime Data { get; set; }
    [Column("inicio")] public DateTime Inicio { get; set; }
    [Column("fim")] public DateTime? Fim { get; set; }
    [Column("dtfrequen")] public DateTime? DtFrequen { get; set; }
    [Column("importado")] public int Importado { get; set; }
    [Column("ERRO")] public int? Erro { get; set; }
    [Column("ERRO2")] public int? Erro2 { get; set; }
    [Column("QTMINDESCFDS")] public int QtMinDescFds { get; set; }
    [Column("INICIO_OLD"), StringLength(5)] public string? InicioOld { get; set; }
    [Column("FIM_OLD"), StringLength(5)] public string? FimOld { get; set; }
}
