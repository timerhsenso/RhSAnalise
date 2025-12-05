using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_FornecedorEmpresa", DisplayName = "Empresa Terceira", CdSistema = "GTR", CdFuncao = "GTR_CAD_EMPRESA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_FornecedorEmpresa")]
public class FornecedorEmpresa
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("IdTipoFornecedor")]
    [Required]
    public int IdTipoFornecedor { get; set; }

    [ForeignKey(nameof(IdTipoFornecedor))]
    public virtual TipoFornecedor TipoFornecedor { get; set; } = null!;

    [Column("CNPJ")]
    [StringLength(18)]
    [Required]
    public string CNPJ { get; set; } = string.Empty;

    [Column("RazaoSocial")]
    [StringLength(200)]
    [Required]
    public string RazaoSocial { get; set; } = string.Empty;

    [Column("NomeFantasia")]
    [StringLength(200)]
    public string? NomeFantasia { get; set; }

    [Column("InscricaoEstadual")]
    [StringLength(20)]
    public string? InscricaoEstadual { get; set; }

    [Column("InscricaoMunicipal")]
    [StringLength(20)]
    public string? InscricaoMunicipal { get; set; }

    [Column("CEP")]
    [StringLength(10)]
    public string? CEP { get; set; }

    [Column("Logradouro")]
    [StringLength(200)]
    public string? Logradouro { get; set; }

    [Column("Numero")]
    [StringLength(20)]
    public string? Numero { get; set; }

    [Column("Complemento")]
    [StringLength(100)]
    public string? Complemento { get; set; }

    [Column("Bairro")]
    [StringLength(100)]
    public string? Bairro { get; set; }

    [Column("Cidade")]
    [StringLength(100)]
    public string? Cidade { get; set; }

    [Column("UF")]
    [StringLength(2)]
    public string? UF { get; set; }

    [Column("Telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("Email")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("Website")]
    [StringLength(200)]
    public string? Website { get; set; }

    [Column("NomeResponsavel")]
    [StringLength(100)]
    public string? NomeResponsavel { get; set; }

    [Column("TelefoneResponsavel")]
    [StringLength(20)]
    public string? TelefoneResponsavel { get; set; }

    [Column("EmailResponsavel")]
    [StringLength(150)]
    public string? EmailResponsavel { get; set; }

    [Column("Observacoes")]
    [StringLength(1000)]
    public string? Observacoes { get; set; }

    [Column("Ativo")]
    public bool Ativo { get; set; } = true;

    [Column("Bloqueado")]
    public bool Bloqueado { get; set; }

    [Column("MotivoBloqueio")]
    [StringLength(500)]
    public string? MotivoBloqueio { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }

    [InverseProperty(nameof(FornecedorColaborador.FornecedorEmpresa))]
    public virtual ICollection<FornecedorColaborador> Colaboradores { get; set; } = new List<FornecedorColaborador>();

    [InverseProperty(nameof(FornecedorContrato.FornecedorEmpresa))]
    public virtual ICollection<FornecedorContrato> Contratos { get; set; } = new List<FornecedorContrato>();

    [InverseProperty(nameof(Veiculo.FornecedorEmpresa))]
    public virtual ICollection<Veiculo> Veiculos { get; set; } = new List<Veiculo>();
}
