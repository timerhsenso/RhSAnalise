using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_VeiculoDocumento", DisplayName = "Documento do Veículo", CdSistema = "GTR", CdFuncao = "GTR_CAD_VEICULODOC", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_VeiculoDocumento")]
public class VeiculoDocumento
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdVeiculo")]
    [Required]
    public int IdVeiculo { get; set; }

    [ForeignKey(nameof(IdVeiculo))]
    public virtual Veiculo Veiculo { get; set; } = null!;

    [Column("IdTipoDocumentoVeiculo")]
    [Required]
    public int IdTipoDocumentoVeiculo { get; set; }

    [ForeignKey(nameof(IdTipoDocumentoVeiculo))]
    public virtual TipoDocumentoVeiculo TipoDocumentoVeiculo { get; set; } = null!;

    [Column("NumeroDocumento")]
    [StringLength(50)]
    public string? NumeroDocumento { get; set; }

    [Column("DataEmissao")]
    public DateTime? DataEmissao { get; set; }

    [Column("DataValidade")]
    public DateTime? DataValidade { get; set; }

    [Column("ArquivoUrl")]
    [StringLength(500)]
    public string? ArquivoUrl { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "PENDENTE";

    [Column("Observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }
}
