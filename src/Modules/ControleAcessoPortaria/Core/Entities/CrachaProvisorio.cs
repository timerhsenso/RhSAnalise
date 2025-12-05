using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_CrachaProvisorio", DisplayName = "Crachá Provisório", CdSistema = "CAP", CdFuncao = "CAP_CAD_CRACHA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_CrachaProvisorio")]
public class CrachaProvisorio
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdPortaria")]
    [Required]
    public int IdPortaria { get; set; }

    [ForeignKey(nameof(IdPortaria))]
    public virtual Portaria Portaria { get; set; } = null!;

    [Column("Numero")]
    [StringLength(20)]
    [Required]
    public string Numero { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(100)]
    public string? Descricao { get; set; }

    [Column("CorCracha")]
    [StringLength(30)]
    public string? CorCracha { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "DISPONIVEL";

    [Column("UltimoUsoEm")]
    public DateTime? UltimoUsoEm { get; set; }

    [Column("UltimoUsuario")]
    [StringLength(150)]
    public string? UltimoUsuario { get; set; }

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

    [InverseProperty(nameof(RegistroAcesso.CrachaProvisorio))]
    public virtual ICollection<RegistroAcesso> RegistrosAcesso { get; set; } = new List<RegistroAcesso>();
}
