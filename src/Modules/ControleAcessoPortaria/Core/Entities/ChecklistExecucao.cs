using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ChecklistExecucao", DisplayName = "Execução de Checklist", CdSistema = "CAP", CdFuncao = "CAP_MOV_CHECKLISTEXEC", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ChecklistExecucao")]
public class ChecklistExecucao
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdChecklistModelo")]
    [Required]
    public int IdChecklistModelo { get; set; }

    [ForeignKey(nameof(IdChecklistModelo))]
    public virtual ChecklistModelo ChecklistModelo { get; set; } = null!;

    [Column("IdRegistroAcesso")]
    public int? IdRegistroAcesso { get; set; }

    [ForeignKey(nameof(IdRegistroAcesso))]
    public virtual RegistroAcesso? RegistroAcesso { get; set; }

    [Column("DataExecucao")]
    [Required]
    public DateTime DataExecucao { get; set; }

    [Column("DataFinalizacao")]
    public DateTime? DataFinalizacao { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "EM_ANDAMENTO";

    [Column("PontuacaoObtida")]
    public int? PontuacaoObtida { get; set; }

    [Column("ExecutadoPor")]
    public Guid? ExecutadoPor { get; set; }

    [Column("NomeExecutor")]
    [StringLength(150)]
    public string? NomeExecutor { get; set; }

    [Column("Observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }

    [InverseProperty(nameof(ChecklistExecucaoItem.ChecklistExecucao))]
    public virtual ICollection<ChecklistExecucaoItem> Itens { get; set; } = new List<ChecklistExecucaoItem>();
}
