using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_Ocorrencia", DisplayName = "Ocorrência", CdSistema = "CAP", CdFuncao = "CAP_MOV_OCORRENCIA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_Ocorrencia")]
public class Ocorrencia
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("NumeroOcorrencia")]
    [StringLength(30)]
    [Required]
    public string NumeroOcorrencia { get; set; } = string.Empty;

    [Column("IdPortaria")]
    [Required]
    public int IdPortaria { get; set; }

    [ForeignKey(nameof(IdPortaria))]
    public virtual Portaria Portaria { get; set; } = null!;

    [Column("IdRegistroAcesso")]
    public int? IdRegistroAcesso { get; set; }

    [ForeignKey(nameof(IdRegistroAcesso))]
    public virtual RegistroAcesso? RegistroAcesso { get; set; }

    [Column("DataHoraOcorrencia")]
    [Required]
    public DateTime DataHoraOcorrencia { get; set; }

    [Column("TipoOcorrencia")]
    [StringLength(50)]
    [Required]
    public string TipoOcorrencia { get; set; } = string.Empty;

    [Column("Gravidade")]
    [StringLength(20)]
    [Required]
    public string Gravidade { get; set; } = "BAIXA";

    [Column("Descricao")]
    [StringLength(2000)]
    [Required]
    public string Descricao { get; set; } = string.Empty;

    [Column("NomePessoaEnvolvida")]
    [StringLength(150)]
    public string? NomePessoaEnvolvida { get; set; }

    [Column("CPFPessoaEnvolvida")]
    [StringLength(14)]
    public string? CPFPessoaEnvolvida { get; set; }

    [Column("EmpresaPessoaEnvolvida")]
    [StringLength(200)]
    public string? EmpresaPessoaEnvolvida { get; set; }

    [Column("PlacaVeiculoEnvolvido")]
    [StringLength(10)]
    public string? PlacaVeiculoEnvolvido { get; set; }

    [Column("LocalOcorrencia")]
    [StringLength(200)]
    public string? LocalOcorrencia { get; set; }

    [Column("AcaoTomada")]
    [StringLength(1000)]
    public string? AcaoTomada { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "ABERTA";

    [Column("RegistradoPor")]
    public Guid? RegistradoPor { get; set; }

    [Column("NomeRegistrador")]
    [StringLength(150)]
    public string? NomeRegistrador { get; set; }

    [Column("FotoEvidenciaUrl")]
    [StringLength(500)]
    public string? FotoEvidenciaUrl { get; set; }

    [Column("Observacoes")]
    [StringLength(1000)]
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
