using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_FornecedorColaborador", DisplayName = "Colaborador Terceiro", CdSistema = "GTR", CdFuncao = "GTR_CAD_COLABORADOR", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_FornecedorColaborador")]
public class FornecedorColaborador
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

    [Column("Nome")]
    [StringLength(150)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("CPF")]
    [StringLength(14)]
    [Required]
    public string CPF { get; set; } = string.Empty;

    [Column("RG")]
    [StringLength(20)]
    public string? RG { get; set; }

    [Column("OrgaoEmissorRG")]
    [StringLength(20)]
    public string? OrgaoEmissorRG { get; set; }

    [Column("DataNascimento")]
    public DateTime? DataNascimento { get; set; }

    [Column("Sexo")]
    [StringLength(1)]
    public string? Sexo { get; set; }

    [Column("IdTipoSanguineo")]
    public int? IdTipoSanguineo { get; set; }

    [Column("Telefone")]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [Column("Celular")]
    [StringLength(20)]
    public string? Celular { get; set; }

    [Column("Email")]
    [StringLength(150)]
    public string? Email { get; set; }

    [Column("Cargo")]
    [StringLength(100)]
    public string? Cargo { get; set; }

    [Column("Funcao")]
    [StringLength(100)]
    public string? Funcao { get; set; }

    [Column("CNH")]
    [StringLength(20)]
    public string? CNH { get; set; }

    [Column("CategoriaCNH")]
    [StringLength(5)]
    public string? CategoriaCNH { get; set; }

    [Column("ValidadeCNH")]
    public DateTime? ValidadeCNH { get; set; }

    [Column("PossuiMOPP")]
    public bool PossuiMOPP { get; set; }

    [Column("ValidadeMOPP")]
    public DateTime? ValidadeMOPP { get; set; }

    [Column("FotoUrl")]
    [StringLength(500)]
    public string? FotoUrl { get; set; }

    [Column("BiometriaDigital")]
    public byte[]? BiometriaDigital { get; set; }

    [Column("BiometriaFacial")]
    public byte[]? BiometriaFacial { get; set; }

    [Column("DataIntegracao")]
    public DateTime? DataIntegracao { get; set; }

    [Column("IntegracaoAprovada")]
    public bool? IntegracaoAprovada { get; set; }

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

    [InverseProperty(nameof(PessoaDocumento.FornecedorColaborador))]
    public virtual ICollection<PessoaDocumento> Documentos { get; set; } = new List<PessoaDocumento>();

    [InverseProperty(nameof(PessoaContato.FornecedorColaborador))]
    public virtual ICollection<PessoaContato> Contatos { get; set; } = new List<PessoaContato>();

    [InverseProperty(nameof(TreinamentoParticipante.FornecedorColaborador))]
    public virtual ICollection<TreinamentoParticipante> Treinamentos { get; set; } = new List<TreinamentoParticipante>();
}
