using System;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

/// <summary>
/// O DDL original não define PK.
/// Para o EF Core é obrigatório definir uma chave; usamos cdtabela como chave natural.
/// </summary>
public class TabelaSalarial
{
    public string CdTabela { get; set; } = default!;
    public string? DcTabela { get; set; }
    public string? FlSeq { get; set; }
    public Guid Id { get; set; }
    public decimal? VlSalInicial { get; set; }
    public decimal? VlSalMediana { get; set; }
    public decimal? VlSalMaximo { get; set; }
    public DateTime? DtValidade { get; set; }
    public Guid? IdTsalValidade { get; set; }
    public decimal? TsalValidadeId { get; set; }
}
