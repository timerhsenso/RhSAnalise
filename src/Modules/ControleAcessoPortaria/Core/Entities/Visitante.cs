using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_Visitante", DisplayName = "Visitante", CdSistema = "CAP", CdFuncao = "CAP_CAD_VISITANTE", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_Visitante")]
public class Visitante
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdTipoPessoa")]
    [Required]
    public int IdTipoPessoa { get; set; }

    [ForeignKey(nameof(IdTipoPessoa))]
    public virtual TipoPessoa TipoPessoa { get; set; } = null!;

    [Column("Nome")]
    [StringLength(150)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("CPF")]
    [StringLength(14)]
    public string? CPF { get; set; }

    [Column("RG")]
    [StringLength(20)]
    public string? RG { get; set; }

    [Column("Empresa")]
    [StringLength(200)]
    public string? Empresa { get; set; }

    [Column("Cargo")]
    [StringLength(100)]
    public string? Cargo { get; set; }

    [Column("Telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("Email")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("FotoUrl")]
    [StringLength(500)]
    public string? FotoUrl { get; set; }

    [Column("Observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

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

    [InverseProperty(nameof(RegistroAcesso.Visitante))]
    public virtual ICollection<RegistroAcesso> RegistrosAcesso { get; set; } = new List<RegistroAcesso>();
}
