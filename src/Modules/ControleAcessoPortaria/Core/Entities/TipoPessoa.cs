using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoPessoa", DisplayName = "Tipo de Pessoa", CdSistema = "CAP", CdFuncao = "CAP_AUX_TIPOPESSOA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoPessoa")]
public class TipoPessoa
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

    [Column("ExigeDocumento")]
    public bool ExigeDocumento { get; set; } = true;

    [Column("ExigeFoto")]
    public bool ExigeFoto { get; set; }

    [Column("ExigeEmpresa")]
    public bool ExigeEmpresa { get; set; }

    [Column("PermiteVeiculoProprio")]
    public bool PermiteVeiculoProprio { get; set; }

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

    [InverseProperty(nameof(Visitante.TipoPessoa))]
    public virtual ICollection<Visitante> Visitantes { get; set; } = new List<Visitante>();
}
