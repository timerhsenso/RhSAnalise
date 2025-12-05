using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

[GenerateCrud(TableName = "SGC_Veiculo", DisplayName = "Veículo", CdSistema = "GTR", CdFuncao = "GTR_CAD_VEICULO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_Veiculo")]
public class Veiculo
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

    [Column("IdTipoVeiculo")]
    [Required]
    public int IdTipoVeiculo { get; set; }

    [ForeignKey(nameof(IdTipoVeiculo))]
    public virtual TipoVeiculo TipoVeiculo { get; set; } = null!;

    [Column("Placa")]
    [StringLength(10)]
    [Required]
    public string Placa { get; set; } = string.Empty;

    [Column("PlacaCarreta")]
    [StringLength(10)]
    public string? PlacaCarreta { get; set; }

    [Column("Marca")]
    [StringLength(50)]
    public string? Marca { get; set; }

    [Column("Modelo")]
    [StringLength(50)]
    public string? Modelo { get; set; }

    [Column("AnoFabricacao")]
    public int? AnoFabricacao { get; set; }

    [Column("AnoModelo")]
    public int? AnoModelo { get; set; }

    [Column("Cor")]
    [StringLength(30)]
    public string? Cor { get; set; }

    [Column("Renavam")]
    [StringLength(20)]
    public string? Renavam { get; set; }

    [Column("Chassi")]
    [StringLength(30)]
    public string? Chassi { get; set; }

    [Column("CapacidadeCarga", TypeName = "decimal(18,2)")]
    public decimal? CapacidadeCarga { get; set; }

    [Column("PesoBrutoTotal", TypeName = "decimal(18,2)")]
    public decimal? PesoBrutoTotal { get; set; }

    [Column("NumeroEixos")]
    public int? NumeroEixos { get; set; }

    [Column("TransportaProdutoPerigoso")]
    public bool TransportaProdutoPerigoso { get; set; }

    [Column("NumeroONU")]
    [StringLength(20)]
    public string? NumeroONU { get; set; }

    [Column("ClasseRisco")]
    [StringLength(50)]
    public string? ClasseRisco { get; set; }

    [Column("Observacoes")]
    [StringLength(500)]
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

    [InverseProperty(nameof(VeiculoDocumento.Veiculo))]
    public virtual ICollection<VeiculoDocumento> Documentos { get; set; } = new List<VeiculoDocumento>();
}
