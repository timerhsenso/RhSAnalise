using System;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

public class MotivoOcorrenciaFrequencia
{
    // PK composta: (TpOcorr, CdMotoc)
    public int TpOcorr { get; set; }
    public string CdMotoc { get; set; } = default!;

    // Demais colunas
    public string? DcMotoc { get; set; }
    public int? FlMovimen { get; set; }
    public string? CdConta { get; set; }
    public int? FlTpFal { get; set; }
    public int? FlExtra { get; set; }
    public int? FlFlAnj { get; set; }
    public int? FlTroca { get; set; }
    public int? FlRegraHe { get; set; }
    public int FlBancoHoras { get; set; }
    public int? TpOcorrLink { get; set; }
    public string? CdMotocLink { get; set; }
    public Guid Id { get; set; }               // default newid()
    public Guid? IdMotivosDeOcorrenciaFrequenciaPai { get; set; }
    public Guid? IdVerba { get; set; }
}
