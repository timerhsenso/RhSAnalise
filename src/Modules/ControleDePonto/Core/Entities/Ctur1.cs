using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Escala por turma e data (sem PK explícita no banco).
/// </summary>
[Table("ctur1")]
[Keyless]
public class Ctur1 : BaseEntity
{
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("cdturma"), StringLength(2)] public string CdTurma { get; set; } = default!;
    [Column("dtcalend")] public DateTime DtCalend { get; set; }
    [Column("hhentrada"), StringLength(5)] public string? HhEntrada { get; set; }
    [Column("hhsaida"), StringLength(5)] public string? HhSaida { get; set; }
    [Column("PONTOREPETICAO"), StringLength(1)] public string? PontoRepeticao { get; set; }
    [Column("idturma")] public Guid? IdTurma { get; set; }
}
