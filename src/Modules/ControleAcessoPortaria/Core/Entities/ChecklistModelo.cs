using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ChecklistModelo", DisplayName = "Modelo de Checklist", CdSistema = "CAP", CdFuncao = "CAP_CAD_CHECKLISTMODELO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ChecklistModelo")]
public class ChecklistModelo
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdTipoChecklist")]
    [Required]
    public int IdTipoChecklist { get; set; }

    [ForeignKey(nameof(IdTipoChecklist))]
    public virtual TipoChecklist TipoChecklist { get; set; } = null!;

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Column("Nome")]
    [StringLength(100)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [Column("Versao")]
    [StringLength(10)]
    public string? Versao { get; set; }

    [Column("PontuacaoMinima")]
    public int? PontuacaoMinima { get; set; }

    [Column("ReprovacaoAutomatica")]
    public bool ReprovacaoAutomatica { get; set; }

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

    [InverseProperty(nameof(ChecklistItem.ChecklistModelo))]
    public virtual ICollection<ChecklistItem> Itens { get; set; } = new List<ChecklistItem>();

    [InverseProperty(nameof(ChecklistExecucao.ChecklistModelo))]
    public virtual ICollection<ChecklistExecucao> Execucoes { get; set; } = new List<ChecklistExecucao>();
}
