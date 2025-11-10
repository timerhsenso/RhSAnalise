// Calnd1.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Primitives;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Datas de calendário/feriados por município.
/// </summary>
[Table("calnd1")]
public class Calnd1 : BaseEntity
{
    /// <summary>Chave técnica gerada por sequencial GUID.</summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("cdmunicip"), StringLength(5)] public string CdMunicip { get; set; } = default!;
    [Column("dtcalend")] public DateTime DtCalend { get; set; }
    [Column("cdferiado"), StringLength(1)] public string CdFeriado { get; set; } = default!;
    [Column("idmunicipio")] public Guid? IdMunicipio { get; set; }
}
