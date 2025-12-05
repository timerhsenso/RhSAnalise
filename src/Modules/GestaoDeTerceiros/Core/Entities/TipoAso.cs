using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoAso", DisplayName = "Tipo de ASO", CdSistema = "GTR", CdFuncao = "GTR_AUX_TIPOASO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoAso")]
public class TipoAso
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

    [Column("Sigla")]
    [StringLength(10)]
    [Required]
    public string Sigla { get; set; } = string.Empty;

    [Column("ValidadeEmMesesPadrao")]
    public int? ValidadeEmMesesPadrao { get; set; }

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
}
