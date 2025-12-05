using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_FornecedorContrato", DisplayName = "Contrato", CdSistema = "GTR", CdFuncao = "GTR_CAD_CONTRATO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_FornecedorContrato")]
public class FornecedorContrato
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdFornecedorEmpresa")]
    [Required]
    public int IdFornecedorEmpresa { get; set; }

    [ForeignKey(nameof(IdFornecedorEmpresa))]
    public virtual FornecedorEmpresa FornecedorEmpresa { get; set; } = null!;

    [Column("NumeroContrato")]
    [StringLength(50)]
    [Required]
    public string NumeroContrato { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(500)]
    public string? Descricao { get; set; }

    [Column("DataInicio")]
    [Required]
    public DateTime DataInicio { get; set; }

    [Column("DataFim")]
    public DateTime? DataFim { get; set; }

    [Column("ValorMensal", TypeName = "decimal(18,2)")]
    public decimal? ValorMensal { get; set; }

    [Column("ValorTotal", TypeName = "decimal(18,2)")]
    public decimal? ValorTotal { get; set; }

    [Column("GestorContrato")]
    [StringLength(100)]
    public string? GestorContrato { get; set; }

    [Column("EmailGestor")]
    [StringLength(150)]
    public string? EmailGestor { get; set; }

    [Column("TelefoneGestor")]
    [StringLength(20)]
    public string? TelefoneGestor { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "VIGENTE";

    [Column("Observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("ArquivoContratoUrl")]
    [StringLength(500)]
    public string? ArquivoContratoUrl { get; set; }

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

    [InverseProperty(nameof(FornecedorContratoServico.FornecedorContrato))]
    public virtual ICollection<FornecedorContratoServico> Servicos { get; set; } = new List<FornecedorContratoServico>();
}
