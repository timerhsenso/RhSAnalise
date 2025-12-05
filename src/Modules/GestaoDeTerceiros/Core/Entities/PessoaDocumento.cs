using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_PessoaDocumento", DisplayName = "Documento", CdSistema = "GTR", CdFuncao = "GTR_CAD_DOCUMENTO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_PessoaDocumento")]
public class PessoaDocumento
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdFornecedorColaborador")]
    [Required]
    public int IdFornecedorColaborador { get; set; }

    [ForeignKey(nameof(IdFornecedorColaborador))]
    public virtual FornecedorColaborador FornecedorColaborador { get; set; } = null!;

    [Column("IdTipoDocumentoTerceiro")]
    [Required]
    public int IdTipoDocumentoTerceiro { get; set; }

    [ForeignKey(nameof(IdTipoDocumentoTerceiro))]
    public virtual TipoDocumentoTerceiro TipoDocumentoTerceiro { get; set; } = null!;

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

    [Column("AprovadoPor")]
    public Guid? AprovadoPor { get; set; }

    [Column("DataAprovacao")]
    public DateTime? DataAprovacao { get; set; }

    [Column("MotivoRejeicao")]
    [StringLength(500)]
    public string? MotivoRejeicao { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }
}
