using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoDocumentoVeiculo", DisplayName = "Tipo de Documento de Veículo", CdSistema = "GTR", CdFuncao = "GTR_AUX_TIPODOCVEICULO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoDocumentoVeiculo")]
public class TipoDocumentoVeiculo
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

    [Column("ValidadeEmMeses")]
    public int? ValidadeEmMeses { get; set; }

    [Column("EhObrigatorio")]
    public bool EhObrigatorio { get; set; }

    [Column("ObrigatorioParaCarga")]
    public bool ObrigatorioParaCarga { get; set; }

    [Column("ObrigatorioParaProdutoPerigoso")]
    public bool ObrigatorioParaProdutoPerigoso { get; set; }

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

    [InverseProperty(nameof(VeiculoDocumento.TipoDocumentoVeiculo))]
    public virtual ICollection<VeiculoDocumento> VeiculosDocumentos { get; set; } = new List<VeiculoDocumento>();
}
