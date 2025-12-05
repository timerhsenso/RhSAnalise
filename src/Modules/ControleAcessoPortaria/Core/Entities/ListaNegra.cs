using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ListaNegra", DisplayName = "Lista Negra", CdSistema = "CAP", CdFuncao = "CAP_CAD_LISTANEGRA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ListaNegra")]
public class ListaNegra
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("TipoRegistro")]
    [StringLength(20)]
    [Required]
    public string TipoRegistro { get; set; } = "PESSOA";

    [Column("NomePessoa")]
    [StringLength(150)]
    public string? NomePessoa { get; set; }

    [Column("CPF")]
    [StringLength(14)]
    public string? CPF { get; set; }

    [Column("RG")]
    [StringLength(20)]
    public string? RG { get; set; }

    [Column("Empresa")]
    [StringLength(200)]
    public string? Empresa { get; set; }

    [Column("CNPJ")]
    [StringLength(18)]
    public string? CNPJ { get; set; }

    [Column("PlacaVeiculo")]
    [StringLength(10)]
    public string? PlacaVeiculo { get; set; }

    [Column("IdMotivoRecusa")]
    [Required]
    public int IdMotivoRecusa { get; set; }

    [ForeignKey(nameof(IdMotivoRecusa))]
    public virtual MotivoRecusa MotivoRecusa { get; set; } = null!;

    [Column("Descricao")]
    [StringLength(1000)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("DataInicioBloqueio")]
    [Required]
    public DateTime DataInicioBloqueio { get; set; }

    [Column("DataFimBloqueio")]
    public DateTime? DataFimBloqueio { get; set; }

    [Column("BloqueioDefinitivo")]
    public bool BloqueioDefinitivo { get; set; }

    [Column("IdOcorrencia")]
    public int? IdOcorrencia { get; set; }

    [ForeignKey(nameof(IdOcorrencia))]
    public virtual Ocorrencia? Ocorrencia { get; set; }

    [Column("IncluidoPor")]
    public Guid? IncluidoPor { get; set; }

    [Column("NomeIncluidor")]
    [StringLength(150)]
    public string? NomeIncluidor { get; set; }

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
