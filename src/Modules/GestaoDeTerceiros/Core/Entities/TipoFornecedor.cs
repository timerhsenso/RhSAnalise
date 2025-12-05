using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(
    TableName = "SGC_TipoFornecedor",
    DisplayName = "Tipo de Fornecedor",
    CdSistema = "GTR",
    CdFuncao = "GTR_AUX_TIPOFORNECEDOR",
    IsLegacyTable = false,
    GenerateApiController = true
)]
[Table("SGC_TipoFornecedor")]
public class TipoFornecedor
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

    [Column("ExigeDocumentacao")]
    public bool ExigeDocumentacao { get; set; } = true;

    [Column("ExigeTreinamento")]
    public bool ExigeTreinamento { get; set; } = true;

    [Column("ExigeASO")]
    public bool ExigeASO { get; set; } = true;

    [Column("ExigeContrato")]
    public bool ExigeContrato { get; set; } = true;

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

    [InverseProperty(nameof(FornecedorEmpresa.TipoFornecedor))]
    public virtual ICollection<FornecedorEmpresa> Empresas { get; set; } = new List<FornecedorEmpresa>();
}
