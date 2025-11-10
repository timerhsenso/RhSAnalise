// Lanc3.cs
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Shared.Core.Primitives;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

/// <summary>
/// Lançamentos por processo/conta (tabela sem PK explícita).
/// </summary>
[Table("lanc3")]
[Keyless]
public class Lanc3 : BaseEntity
{
    [Column("nomatric"), StringLength(8)] public string NoMatric { get; set; } = default!;
    [Column("cdempresa")] public int CdEmpresa { get; set; }
    [Column("cdfilial")] public int CdFilial { get; set; }
    [Column("noprocesso"), StringLength(6)] public string NoProcesso { get; set; } = default!;
    [Column("cdconta"), StringLength(4)] public string CdConta { get; set; } = default!;
    [Column("cdccusres"), StringLength(5)] public string? CdCcUsRes { get; set; }
    [Column("qtconta")] public float? QtConta { get; set; } // real -> float
    [Column("cdusuario"), StringLength(20)] public string? CdUsuario { get; set; }
}
