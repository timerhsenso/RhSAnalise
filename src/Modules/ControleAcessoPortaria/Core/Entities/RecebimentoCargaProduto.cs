using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_RecebimentoCargaProduto", DisplayName = "Produto da Carga", CdSistema = "CAP", CdFuncao = "CAP_MOV_RECEBIMENTOCARGAPRODUTO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_RecebimentoCargaProduto")]
public class RecebimentoCargaProduto
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdRecebimentoCarga")]
    [Required]
    public int IdRecebimentoCarga { get; set; }

    [ForeignKey(nameof(IdRecebimentoCarga))]
    public virtual RecebimentoCarga RecebimentoCarga { get; set; } = null!;

    [Column("CodigoProduto")]
    [StringLength(50)]
    public string? CodigoProduto { get; set; }

    [Column("DescricaoProduto")]
    [StringLength(300)]
    [Required]
    public string DescricaoProduto { get; set; } = string.Empty;

    [Column("Unidade")]
    [StringLength(20)]
    public string? Unidade { get; set; }

    [Column("QuantidadeNF", TypeName = "decimal(18,4)")]
    public decimal? QuantidadeNF { get; set; }

    [Column("QuantidadeRecebida", TypeName = "decimal(18,4)")]
    public decimal? QuantidadeRecebida { get; set; }

    [Column("QuantidadeAceita", TypeName = "decimal(18,4)")]
    public decimal? QuantidadeAceita { get; set; }

    [Column("QuantidadeRecusada", TypeName = "decimal(18,4)")]
    public decimal? QuantidadeRecusada { get; set; }

    [Column("ValorUnitario", TypeName = "decimal(18,4)")]
    public decimal? ValorUnitario { get; set; }

    [Column("ValorTotal", TypeName = "decimal(18,2)")]
    public decimal? ValorTotal { get; set; }

    [Column("NumeroLote")]
    [StringLength(50)]
    public string? NumeroLote { get; set; }

    [Column("DataFabricacao")]
    public DateTime? DataFabricacao { get; set; }

    [Column("DataValidade")]
    public DateTime? DataValidade { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "PENDENTE";

    [Column("IdMotivoRecusa")]
    public int? IdMotivoRecusa { get; set; }

    [ForeignKey(nameof(IdMotivoRecusa))]
    public virtual MotivoRecusa? MotivoRecusa { get; set; }

    [Column("ObservacaoRecusa")]
    [StringLength(500)]
    public string? ObservacaoRecusa { get; set; }

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
