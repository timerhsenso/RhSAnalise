// Chor2.cs
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Variações por dia da semana para um horário administrativo.
/// </summary>
[Table("CHOR2")]
[PrimaryKey(nameof(CdCargHor), nameof(DiaDaSemana))]
public class Chor2 : BaseEntity
{
    [Column("CDCARGHOR"), StringLength(2)] public string CdCargHor { get; set; } = default!;
    [Column("DIADASEMANA")] public int DiaDaSemana { get; set; }

    [Column("HHENTRADA"), StringLength(5)] public string HhEntrada { get; set; } = default!;
    [Column("HHSAIDA"), StringLength(5)] public string HhSaida { get; set; } = default!;
    [Column("HHINIINT"), StringLength(5)] public string HhIniInt { get; set; } = default!;
    [Column("HHFIMINT"), StringLength(5)] public string HhFimInt { get; set; } = default!;
    [Column("FLHABILITADO")] public int FlHabilitado { get; set; }

    [Column("codhors1050"), StringLength(30)] public string? CodHors1050 { get; set; }
    [Column("id")] public Guid Id { get; set; }
    [Column("idhorarioadministrativo")] public Guid? IdHorarioAdministrativo { get; set; }
}
