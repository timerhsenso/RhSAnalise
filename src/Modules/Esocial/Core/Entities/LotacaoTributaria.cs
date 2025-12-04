using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Lotação Tributária - Evento S-1020 do eSocial
/// Identifica a classificação da atividade para fins de atribuição do 
/// código FPAS, RAT e contribuições para Terceiros.
/// Tabela: lotacoestributarias
/// </summary>
/// <remarks>
/// Filiais do módulo GestaoDePessoas referenciam esta tabela via IdLotacaoTributaria (Guid).
/// Para listar filiais desta lotação, usar IGestaoDePessoasLookupService.
/// </remarks>
[GenerateCrud(
    TableName = "lotacoestributarias",
    DisplayName = "Lotação Tributária",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_LOTACAO",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("lotacoestributarias")]
public class LotacaoTributaria
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("codlotacao")]
    [StringLength(30)]
    [Display(Name = "Código da Lotação")]
    public string CodLotacao { get; set; } = string.Empty;

    [Column("descricao")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Descricao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // FK para Tabela 10 - Tipo de Lotação (DENTRO do mesmo módulo) ✅
    // NOTA: FK é string (código), não Guid
    // ═══════════════════════════════════════════════════════════════════

    [Column("tplotacao")]
    [StringLength(2)]
    [Display(Name = "Tipo de Lotação")]
    public string TpLotacao { get; set; } = string.Empty;

    [ForeignKey(nameof(TpLotacao))]
    public virtual EsocialTabela10? Tabela10 { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // FK para Tabela 4 - FPAS (DENTRO do mesmo módulo) ✅
    // NOTA: FK é string (código), não Guid
    // ═══════════════════════════════════════════════════════════════════

    [Column("fpas")]
    [StringLength(3)]
    [Display(Name = "FPAS")]
    public string Fpas { get; set; } = string.Empty;

    [ForeignKey(nameof(Fpas))]
    public virtual EsocialTabela4? Tabela4 { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados de Inscrição
    // ═══════════════════════════════════════════════════════════════════

    [Column("tpinsc")]
    [StringLength(1)]
    [Display(Name = "Tipo de Inscrição")]
    public string? TpInsc { get; set; }

    [Column("nrinsc")]
    [StringLength(14)]
    [Display(Name = "Número de Inscrição")]
    public string? NrInsc { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Terceiros
    // ═══════════════════════════════════════════════════════════════════

    [Column("codtercs")]
    [StringLength(4)]
    [Display(Name = "Código Terceiros")]
    public string CodTercs { get; set; } = string.Empty;

    [Column("codtercssusp")]
    [StringLength(4)]
    [Display(Name = "Código Terceiros Suspenso")]
    public string? CodTercsSusp { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados do Contratante
    // ═══════════════════════════════════════════════════════════════════

    [Column("tpinsccontrat")]
    [Display(Name = "Tipo Inscrição Contratante")]
    public short? TpInscContrat { get; set; }

    [Column("nrInscContrat")]
    [StringLength(14)]
    [Display(Name = "Nr Inscrição Contratante")]
    public string? NrInscContrat { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Dados do Proprietário
    // ═══════════════════════════════════════════════════════════════════

    [Column("tpinscprop")]
    [Display(Name = "Tipo Inscrição Proprietário")]
    public short? TpInscProp { get; set; }

    [Column("nrinscprop")]
    [StringLength(14)]
    [Display(Name = "Nr Inscrição Proprietário")]
    public string? NrInscProp { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // Alíquotas
    // ═══════════════════════════════════════════════════════════════════

    [Column("aliqRat")]
    [StringLength(1)]
    [Display(Name = "Alíquota RAT")]
    public string? AliqRat { get; set; }

    [Column("fap")]
    [Display(Name = "FAP")]
    public double? Fap { get; set; }

    // ═══════════════════════════════════════════════════════════════════
    // ❌ SEM Collections de OUTRO módulo (GestaoDePessoas)
    // Filial referencia esta tabela via IdLotacaoTributaria (Guid)
    // Para buscar filiais: IGestaoDePessoasLookupService.GetFiliaisByLotacao()
    // ═══════════════════════════════════════════════════════════════════
}
