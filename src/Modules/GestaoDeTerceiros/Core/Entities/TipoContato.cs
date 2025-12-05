using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoContato", DisplayName = "Tipo de Contato", CdSistema = "GTR", CdFuncao = "GTR_AUX_TIPOCONTATO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoContato")]
public class TipoContato
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
    [StringLength(50)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("Mascara")]
    [StringLength(30)]
    public string? Mascara { get; set; }

    [Column("RegexValidacao")]
    [StringLength(200)]
    public string? RegexValidacao { get; set; }

    [Column("Icone")]
    [StringLength(50)]
    public string? Icone { get; set; }

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
