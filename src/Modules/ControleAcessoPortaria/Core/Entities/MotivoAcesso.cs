using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_MotivoAcesso", DisplayName = "Motivo de Acesso", CdSistema = "CAP", CdFuncao = "CAP_AUX_MOTIVOACESSO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_MotivoAcesso")]
public class MotivoAcesso
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(100)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("ExigeDestino")]
    public bool ExigeDestino { get; set; }

    [Column("ExigeResponsavel")]
    public bool ExigeResponsavel { get; set; }

    [Column("ExigeNotaFiscal")]
    public bool ExigeNotaFiscal { get; set; }

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

    [InverseProperty(nameof(RegistroAcesso.MotivoAcesso))]
    public virtual ICollection<RegistroAcesso> RegistrosAcesso { get; set; } = new List<RegistroAcesso>();
}
