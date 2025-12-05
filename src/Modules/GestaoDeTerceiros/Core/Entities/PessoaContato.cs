using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_PessoaContato", DisplayName = "Contato de Emergência", CdSistema = "GTR", CdFuncao = "GTR_CAD_CONTATO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_PessoaContato")]
public class PessoaContato
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdFornecedorColaborador")]
    [Required]
    public int IdFornecedorColaborador { get; set; }

    [ForeignKey(nameof(IdFornecedorColaborador))]
    public virtual FornecedorColaborador FornecedorColaborador { get; set; } = null!;

    [Column("IdTipoParentesco")]
    public int? IdTipoParentesco { get; set; }

    [ForeignKey(nameof(IdTipoParentesco))]
    public virtual TipoParentesco? TipoParentesco { get; set; }

    [Column("Nome")]
    [StringLength(150)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("Telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("Celular")]
    [StringLength(20)]
    public string? Celular { get; set; }

    [Column("Email")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("EhContatoPrincipal")]
    public bool EhContatoPrincipal { get; set; }

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
}
