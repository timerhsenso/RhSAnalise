using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_TipoDocumentoTerceiro", DisplayName = "Tipo de Documento", CdSistema = "GTR", CdFuncao = "GTR_AUX_TIPODOCUMENTO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_TipoDocumentoTerceiro")]
public class TipoDocumentoTerceiro
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

    [Column("ValidadeEmMesesPadrao")]
    public int? ValidadeEmMesesPadrao { get; set; }

    [Column("EhObrigatorio")]
    public bool EhObrigatorio { get; set; }

    [Column("ExigeAprovacao")]
    public bool ExigeAprovacao { get; set; } = true;

    [Column("PermiteMultiplos")]
    public bool PermiteMultiplos { get; set; }

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

    [InverseProperty(nameof(PessoaDocumento.TipoDocumentoTerceiro))]
    public virtual ICollection<PessoaDocumento> PessoasDocumentos { get; set; } = new List<PessoaDocumento>();
}
