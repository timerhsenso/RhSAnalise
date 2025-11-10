using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.ControleDePonto.Core.Entities;

/// <summary>
/// Marcações de ponto (batidas) por funcionário.
/// </summary>
[Table("BATIDAS")]
[PrimaryKey(nameof(CdEmpresa), nameof(CdFilial), nameof(NoMatric), nameof(Data), nameof(Hora))]
public class Batidas : BaseEntity
{
    [Column("CDEMPRESA")] public int CdEmpresa { get; set; }
    [Column("CDFILIAL")] public int CdFilial { get; set; }
    [Column("NOMATRIC"), StringLength(8)] public string NoMatric { get; set; } = default!;
    [Column("DATA")] public DateTime Data { get; set; }
    [Column("HORA"), StringLength(5)] public string Hora { get; set; } = default!;
    [Column("TIPO"), StringLength(2)] public string Tipo { get; set; } = default!;
    [Column("ERRO"), StringLength(10)] public string Erro { get; set; } = "0000000000";
    [Column("IMPORTADO")] public int Importado { get; set; }
    [Column("MOTIVO"), StringLength(200)] public string? Motivo { get; set; }
    [Column("id_guid")] public Guid IdGuid { get; set; }
    [Column("idfuncionario")] public Guid? IdFuncionario { get; set; }
}
