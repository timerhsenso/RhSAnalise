using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoTreinamento", DisplayName = "Tipo de Treinamento", CdSistema = "GTR", CdFuncao = "GTR_AUX_TIPOTREINAMENTO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoTreinamento")]
public class TipoTreinamento
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
    [StringLength(200)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("CargaHoraria", TypeName = "decimal(5,2)")]
    public decimal? CargaHoraria { get; set; }

    [Column("ValidadeEmMeses")]
    public int? ValidadeEmMeses { get; set; }

    [Column("EhObrigatorio")]
    public bool EhObrigatorio { get; set; }

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

    [InverseProperty(nameof(TreinamentoTurma.TipoTreinamento))]
    public virtual ICollection<TreinamentoTurma> Turmas { get; set; } = new List<TreinamentoTurma>();
}
