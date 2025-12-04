using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Motivos de Ocorrência para controle de frequência
/// Tabela legada: mfre1
/// </summary>
/// <remarks>
/// ⚠️ ATENÇÃO: Esta tabela tem PK COMPOSTA (TpOcorr, CdMotoc)
/// O campo Id é apenas uma chave alternativa (Unique).
/// Configuração feita manualmente no EsocialDbContext.
/// 
/// Filiais do módulo GestaoDePessoas referenciam esta tabela via chave composta
/// (TpOcorrHE + CdMotocHE, TpOcorrFalta + CdMotocFalta, etc.)
/// </remarks>
[GenerateCrud(
    TableName = "mfre1",
    DisplayName = "Motivo de Ocorrência",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_MOTOC",
    IsLegacyTable = true,
    GenerateApiController = true,
    GenerateEfConfig = false  // ← PK composta configurada no DbContext
)]
[Table("mfre1")]
public class EsocialMotivoOcorrencia
{
    // ═══════════════════════════════════════════════════════════════════
    // CHAVE PRIMÁRIA COMPOSTA
    // Configurada no DbContext: HasKey(e => new { e.TpOcorr, e.CdMotoc })
    // ═══════════════════════════════════════════════════════════════════

    [Column("tpocorr")]
    [Display(Name = "Tipo de Ocorrência")]
    public int TpOcorr { get; set; }

    [Column("cdmotoc")]
    [StringLength(4)]
    [Display(Name = "Código")]
    public string CdMotoc { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // CHAVE ALTERNATIVA (Unique, mas NÃO é a PK)
    // ═══════════════════════════════════════════════════════════════════

    [Column("id")]
    [Display(Name = "Id")]
    public Guid Id { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados da Ocorrência
    // ═══════════════════════════════════════════════════════════════════

    [Column("dcmotoc")]
    [StringLength(40)]
    [Display(Name = "Descrição")]
    public string? DcMotoc { get; set; }

    [Column("flmovimen")]
    [Display(Name = "Flag Movimentação")]
    public int? FlMovimen { get; set; }

    [Column("cdconta")]
    [StringLength(4)]
    [Display(Name = "Código Conta")]
    public string? CdConta { get; set; }

    [Column("fltpfal")]
    [Display(Name = "Flag Tipo Falta")]
    public int? FlTpFal { get; set; }

    [Column("flextra")]
    [Display(Name = "Flag Extra")]
    public int? FlExtra { get; set; }

    [Column("flflanj")]
    [Display(Name = "Flag Lançamento")]
    public int? FlFlAnj { get; set; }

    [Column("FLTROCA")]
    [Display(Name = "Flag Troca")]
    public int? FlTroca { get; set; }

    [Column("FLREGRAHE")]
    [Display(Name = "Flag Regra HE")]
    public int? FlRegraHE { get; set; }

    [Column("FLBANCOHORAS")]
    [Display(Name = "Flag Banco de Horas")]
    public int FlBancoHoras { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Link para outro Motivo (via chave composta legada)
    // ═══════════════════════════════════════════════════════════════════

    [Column("TPOCORRLINK")]
    [Display(Name = "Tipo Ocorrência Link")]
    public int? TpOcorrLink { get; set; }

    [Column("CDMOTOCLINK")]
    [StringLength(4)]
    [Display(Name = "Código Motivo Link")]
    public string? CdMotocLink { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Auto-referência (Motivo Pai) - via Guid
    // Configurada no DbContext com HasPrincipalKey(e => e.Id)
    // ═══════════════════════════════════════════════════════════════════

    [Column("idmotivosdeocorrenciafrequenciapai")]
    [Display(Name = "Motivo Pai")]
    public Guid? IdMotivoPai { get; set; }

    [ForeignKey(nameof(IdMotivoPai))]
    public virtual EsocialMotivoOcorrencia? MotivoPai { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FK para Verba (módulo Folha/Frequência)
    // Apenas o ID - sem navigation (cross-module)
    // ═══════════════════════════════════════════════════════════════════

    [Column("idverba")]
    [Display(Name = "Verba Associada")]
    public Guid? IdVerba { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // ❌ SEM Collections de OUTRO módulo (GestaoDePessoas)
    // Filial referencia via chave composta (TpOcorrHE + CdMotocHE, etc.)
    // Para buscar filiais: IGestaoDePessoasLookupService
    // ═══════════════════════════════════════════════════════════════════
}