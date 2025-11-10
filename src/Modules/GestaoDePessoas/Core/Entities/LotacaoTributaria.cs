using System;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

public class LotacaoTributaria
{
    // PK
    public Guid Id { get; set; }

    // Colunas
    public string CodLotacao { get; set; } = default!;
    public string Descricao { get; set; } = default!;
    public string TpLotacao { get; set; } = default!;
    public string? TpInsc { get; set; }
    public string? NrInsc { get; set; }
    public string FPAS { get; set; } = default!;
    public string CodTercs { get; set; } = default!;
    public string? CodTercsSusp { get; set; }
    public short? TpInscContrat { get; set; }
    public string? NrInscContrat { get; set; }
    public short? TpInscProp { get; set; }
    public string? NrInscProp { get; set; }
    public string? AliqRat { get; set; }
    public double? Fap { get; set; }

    // FKs comentadas até existir o mapeamento das tabelas de apoio:
    // public Tab10Esocial? Tab10 { get; set; }   // FK tplotacao -> tab10_esocial.tab10_codigo
    // public Tab4Esocial?  Tab4  { get; set; }   // FK fpas     -> tab4_esocial.tab4_codigo
}
