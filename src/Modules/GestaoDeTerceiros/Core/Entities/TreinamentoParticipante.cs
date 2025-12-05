using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TreinamentoParticipante", DisplayName = "Participante", CdSistema = "GTR", CdFuncao = "GTR_CAD_PARTICIPANTE", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TreinamentoParticipante")]
public class TreinamentoParticipante
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdTreinamentoTurma")]
    [Required]
    public int IdTreinamentoTurma { get; set; }

    [ForeignKey(nameof(IdTreinamentoTurma))]
    public virtual TreinamentoTurma TreinamentoTurma { get; set; } = null!;

    [Column("IdFornecedorColaborador")]
    [Required]
    public int IdFornecedorColaborador { get; set; }

    [ForeignKey(nameof(IdFornecedorColaborador))]
    public virtual FornecedorColaborador FornecedorColaborador { get; set; } = null!;

    [Column("Presente")]
    public bool? Presente { get; set; }

    [Column("Aprovado")]
    public bool? Aprovado { get; set; }

    [Column("Nota", TypeName = "decimal(5,2)")]
    public decimal? Nota { get; set; }

    [Column("CargaHorariaRealizada", TypeName = "decimal(5,2)")]
    public decimal? CargaHorariaRealizada { get; set; }

    [Column("NumeroCertificado")]
    [StringLength(50)]
    public string? NumeroCertificado { get; set; }

    [Column("DataEmissaoCertificado")]
    public DateTime? DataEmissaoCertificado { get; set; }

    [Column("DataValidadeCertificado")]
    public DateTime? DataValidadeCertificado { get; set; }

    [Column("CertificadoUrl")]
    [StringLength(500)]
    public string? CertificadoUrl { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "INSCRITO";

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
