using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_MotivoRecusa", DisplayName = "Motivo de Recusa", CdSistema = "CAP", CdFuncao = "CAP_AUX_MOTIVORECUSA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_MotivoRecusa")]
public class MotivoRecusa
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("Tipo")]
    [StringLength(20)]
    [Required]
    public string Tipo { get; set; } = "ACESSO";

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(200)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("GeraOcorrencia")]
    public bool GeraOcorrencia { get; set; }

    [Column("BloqueioTemporario")]
    public bool BloqueioTemporario { get; set; }

    [Column("DiasBloqueio")]
    public int? DiasBloqueio { get; set; }

    [Column("Ordem")]
    public int Ordem { get; set; }

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
}
