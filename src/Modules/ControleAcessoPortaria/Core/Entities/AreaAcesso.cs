using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_AreaAcesso", 
    DisplayName = "Área de Acesso", 
    CdSistema = "CAP", 
    CdFuncao = "CAP_CAD_AREAACESSO", 
    IsLegacyTable = false, 
    GenerateApiController = true)]
[Table("SGC_AreaAcesso")]
public class AreaAcesso
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

    [Column("Nome")]
    [StringLength(100)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("NivelRestricao")]
    [StringLength(20)]
    [Required]
    public string NivelRestricao { get; set; } = "CONTROLADO";

    [Column("ExigeEPI")]
    public bool ExigeEPI { get; set; }

    [Column("ExigeTreinamento")]
    public bool ExigeTreinamento { get; set; }

    [Column("TreinamentosExigidos")]
    [StringLength(500)]
    public string? TreinamentosExigidos { get; set; }

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

    [InverseProperty(nameof(RegistroAcesso.AreaDestino))]
    public virtual ICollection<RegistroAcesso> RegistrosAcesso { get; set; } = new List<RegistroAcesso>();
}
