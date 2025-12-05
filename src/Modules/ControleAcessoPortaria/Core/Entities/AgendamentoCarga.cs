using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_AgendamentoCarga", DisplayName = "Agendamento de Carga", CdSistema = "CAP", CdFuncao = "CAP_CAD_AGENDAMENTOCARGA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_AgendamentoCarga")]
public class AgendamentoCarga
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("NumeroAgendamento")]
    [StringLength(30)]
    [Required]
    public string NumeroAgendamento { get; set; } = string.Empty;

    [Column("IdFornecedorEmpresa")]
    public int? IdFornecedorEmpresa { get; set; }

    [Column("NomeFornecedor")]
    [StringLength(200)]
    [Required]
    public string NomeFornecedor { get; set; } = string.Empty;

    [Column("CNPJFornecedor")]
    [StringLength(18)]
    public string? CNPJFornecedor { get; set; }

    [Column("DataAgendamento")]
    [Required]
    public DateTime DataAgendamento { get; set; }

    [Column("HoraInicio")]
    [StringLength(5)]
    public string? HoraInicio { get; set; }

    [Column("HoraFim")]
    [StringLength(5)]
    public string? HoraFim { get; set; }

    [Column("TipoCarga")]
    [StringLength(20)]
    [Required]
    public string TipoCarga { get; set; } = "RECEBIMENTO";

    [Column("DescricaoCarga")]
    [StringLength(500)]
    public string? DescricaoCarga { get; set; }

    [Column("NumeroNotaFiscal")]
    [StringLength(50)]
    public string? NumeroNotaFiscal { get; set; }

    [Column("NumeroPedido")]
    [StringLength(50)]
    public string? NumeroPedido { get; set; }

    [Column("PesoEstimado", TypeName = "decimal(18,2)")]
    public decimal? PesoEstimado { get; set; }

    [Column("QuantidadeVolumes")]
    public int? QuantidadeVolumes { get; set; }

    [Column("PlacaVeiculo")]
    [StringLength(10)]
    public string? PlacaVeiculo { get; set; }

    [Column("NomeMotorista")]
    [StringLength(150)]
    public string? NomeMotorista { get; set; }

    [Column("SetorSolicitante")]
    [StringLength(100)]
    public string? SetorSolicitante { get; set; }

    [Column("ResponsavelRecebimento")]
    [StringLength(150)]
    public string? ResponsavelRecebimento { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "AGENDADO";

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

    [InverseProperty(nameof(RecebimentoCarga.AgendamentoCarga))]
    public virtual ICollection<RecebimentoCarga> Recebimentos { get; set; } = new List<RecebimentoCarga>();
}
