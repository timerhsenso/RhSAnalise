using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ChecklistExecucaoItem", DisplayName = "Resposta de Checklist", CdSistema = "CAP", CdFuncao = "CAP_MOV_CHECKLISTEXECITEM", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ChecklistExecucaoItem")]
public class ChecklistExecucaoItem
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdChecklistExecucao")]
    [Required]
    public int IdChecklistExecucao { get; set; }

    [ForeignKey(nameof(IdChecklistExecucao))]
    public virtual ChecklistExecucao ChecklistExecucao { get; set; } = null!;

    [Column("IdChecklistItem")]
    [Required]
    public int IdChecklistItem { get; set; }

    [ForeignKey(nameof(IdChecklistItem))]
    public virtual ChecklistItem ChecklistItem { get; set; } = null!;

    [Column("Resposta")]
    [StringLength(500)]
    public string? Resposta { get; set; }

    [Column("RespostaBoolean")]
    public bool? RespostaBoolean { get; set; }

    [Column("RespostaNumero", TypeName = "decimal(18,4)")]
    public decimal? RespostaNumero { get; set; }

    [Column("RespostaData")]
    public DateTime? RespostaData { get; set; }

    [Column("EhConforme")]
    public bool? EhConforme { get; set; }

    [Column("PontuacaoObtida")]
    public int? PontuacaoObtida { get; set; }

    [Column("FotoEvidenciaUrl")]
    [StringLength(500)]
    public string? FotoEvidenciaUrl { get; set; }

    [Column("Observacao")]
    [StringLength(500)]
    public string? Observacao { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }
}
