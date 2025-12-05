using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TreinamentoTurma", DisplayName = "Turma de Treinamento", CdSistema = "GTR", CdFuncao = "GTR_CAD_TURMA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TreinamentoTurma")]
public class TreinamentoTurma
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdTipoTreinamento")]
    [Required]
    public int IdTipoTreinamento { get; set; }

    [ForeignKey(nameof(IdTipoTreinamento))]
    public virtual TipoTreinamento TipoTreinamento { get; set; } = null!;

    [Column("CodigoTurma")]
    [StringLength(30)]
    [Required]
    public string CodigoTurma { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("DataInicio")]
    [Required]
    public DateTime DataInicio { get; set; }

    [Column("DataFim")]
    public DateTime? DataFim { get; set; }

    [Column("HoraInicio")]
    [StringLength(5)]
    public string? HoraInicio { get; set; }

    [Column("HoraFim")]
    [StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("CargaHoraria", TypeName = "decimal(5,2)")]
    public decimal? CargaHoraria { get; set; }

    [Column("Local")]
    [StringLength(200)]
    public string? Local { get; set; }

    [Column("Instrutor")]
    [StringLength(150)]
    public string? Instrutor { get; set; }

    [Column("EntidadeCertificadora")]
    [StringLength(200)]
    public string? EntidadeCertificadora { get; set; }

    [Column("VagasDisponiveis")]
    public int? VagasDisponiveis { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "AGENDADA";

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

    [InverseProperty(nameof(TreinamentoParticipante.TreinamentoTurma))]
    public virtual ICollection<TreinamentoParticipante> Participantes { get; set; } = new List<TreinamentoParticipante>();
}
