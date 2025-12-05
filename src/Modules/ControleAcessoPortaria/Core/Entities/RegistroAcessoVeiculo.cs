using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_RegistroAcessoVeiculo", DisplayName = "Veículo no Acesso", CdSistema = "CAP", CdFuncao = "CAP_MOV_REGISTROACESSOVEICULO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_RegistroAcessoVeiculo")]
public class RegistroAcessoVeiculo
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdRegistroAcesso")]
    [Required]
    public int IdRegistroAcesso { get; set; }

    [ForeignKey(nameof(IdRegistroAcesso))]
    public virtual RegistroAcesso RegistroAcesso { get; set; } = null!;

    [Column("IdVeiculo")]
    public int? IdVeiculo { get; set; }

    [Column("Placa")]
    [StringLength(10)]
    [Required]
    public string Placa { get; set; } = string.Empty;

    [Column("PlacaCarreta")]
    [StringLength(10)]
    public string? PlacaCarreta { get; set; }

    [Column("TipoVeiculo")]
    [StringLength(50)]
    public string? TipoVeiculo { get; set; }

    [Column("Marca")]
    [StringLength(50)]
    public string? Marca { get; set; }

    [Column("Modelo")]
    [StringLength(50)]
    public string? Modelo { get; set; }

    [Column("Cor")]
    [StringLength(30)]
    public string? Cor { get; set; }

    [Column("NomeMotorista")]
    [StringLength(150)]
    public string? NomeMotorista { get; set; }

    [Column("CNHMotorista")]
    [StringLength(20)]
    public string? CNHMotorista { get; set; }

    [Column("PesoEntrada", TypeName = "decimal(18,2)")]
    public decimal? PesoEntrada { get; set; }

    [Column("PesoSaida", TypeName = "decimal(18,2)")]
    public decimal? PesoSaida { get; set; }

    [Column("PesoLiquido", TypeName = "decimal(18,2)")]
    public decimal? PesoLiquido { get; set; }

    [Column("KmEntrada")]
    public int? KmEntrada { get; set; }

    [Column("KmSaida")]
    public int? KmSaida { get; set; }

    [Column("NumeroNotaFiscal")]
    [StringLength(50)]
    public string? NumeroNotaFiscal { get; set; }

    [Column("NumeroLacreEntrada")]
    [StringLength(50)]
    public string? NumeroLacreEntrada { get; set; }

    [Column("NumeroLacreSaida")]
    [StringLength(50)]
    public string? NumeroLacreSaida { get; set; }

    [Column("Observacoes")]
    [StringLength(500)]
    public string? Observacoes { get; set; }

    [Column("Aud_CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [Column("Aud_UpdatedAt")]
    public DateTime? UpdatedAt { get; set; }

    [Column("Aud_IdUsuarioCadastro")]
    public Guid? CreatedBy { get; set; }

    [Column("Aud_IdUsuarioAtualizacao")]
    public Guid? UpdatedBy { get; set; }
}
