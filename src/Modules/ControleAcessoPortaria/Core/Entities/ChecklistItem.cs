using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ChecklistItem", DisplayName = "Item de Checklist", CdSistema = "CAP", CdFuncao = "CAP_CAD_CHECKLISTITEM", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ChecklistItem")]
public class ChecklistItem
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdChecklistModelo")]
    [Required]
    public int IdChecklistModelo { get; set; }

    [ForeignKey(nameof(IdChecklistModelo))]
    public virtual ChecklistModelo ChecklistModelo { get; set; } = null!;

    [Column("Ordem")]
    [Required]
    public int Ordem { get; set; }

    [Column("Pergunta")]
    [StringLength(500)]
    [Required]
    public string Pergunta { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [Column("TipoResposta")]
    [StringLength(20)]
    [Required]
    public string TipoResposta { get; set; } = "SIM_NAO";

    [Column("OpcoesResposta")]
    [StringLength(500)]
    public string? OpcoesResposta { get; set; }

    [Column("EhObrigatorio")]
    public bool EhObrigatorio { get; set; } = true;

    [Column("EhCritico")]
    public bool EhCritico { get; set; }

    [Column("RespostaQueReprova")]
    [StringLength(50)]
    public string? RespostaQueReprova { get; set; }

    [Column("Pontuacao")]
    public int? Pontuacao { get; set; }

    [Column("Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }

    [InverseProperty(nameof(ChecklistExecucaoItem.ChecklistItem))]
    public virtual ICollection<ChecklistExecucaoItem> ExecucoesItens { get; set; } = new List<ChecklistExecucaoItem>();
}
