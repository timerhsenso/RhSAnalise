using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_FornecedorContratoServico", DisplayName = "Serviço do Contrato", CdSistema = "GTR", CdFuncao = "GTR_CAD_CONTRATOSERVICO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_FornecedorContratoServico")]
public class FornecedorContratoServico
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdFornecedorContrato")]
    [Required]
    public int IdFornecedorContrato { get; set; }

    [ForeignKey(nameof(IdFornecedorContrato))]
    public virtual FornecedorContrato FornecedorContrato { get; set; } = null!;

    [Column("Descricao")]
    [StringLength(300)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("Unidade")]
    [StringLength(20)]
    public string? Unidade { get; set; }

    [Column("Quantidade", TypeName = "decimal(18,4)")]
    public decimal? Quantidade { get; set; }

    [Column("ValorUnitario", TypeName = "decimal(18,4)")]
    public decimal? ValorUnitario { get; set; }

    [Column("ValorTotal", TypeName = "decimal(18,2)")]
    public decimal? ValorTotal { get; set; }

    [Column("LocalExecucao")]
    [StringLength(200)]
    public string? LocalExecucao { get; set; }

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
}
