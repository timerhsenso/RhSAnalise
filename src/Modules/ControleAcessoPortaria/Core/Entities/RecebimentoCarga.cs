using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_RecebimentoCarga", DisplayName = "Recebimento de Carga", CdSistema = "CAP", CdFuncao = "CAP_MOV_RECEBIMENTOCARGA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_RecebimentoCarga")]
public class RecebimentoCarga
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("NumeroRecebimento")]
    [StringLength(30)]
    [Required]
    public string NumeroRecebimento { get; set; } = string.Empty;

    [Column("IdAgendamentoCarga")]
    public int? IdAgendamentoCarga { get; set; }

    [ForeignKey(nameof(IdAgendamentoCarga))]
    public virtual AgendamentoCarga? AgendamentoCarga { get; set; }

    [Column("IdRegistroAcesso")]
    public int? IdRegistroAcesso { get; set; }

    [ForeignKey(nameof(IdRegistroAcesso))]
    public virtual RegistroAcesso? RegistroAcesso { get; set; }

    [Column("IdPortaria")]
    [Required]
    public int IdPortaria { get; set; }

    [ForeignKey(nameof(IdPortaria))]
    public virtual Portaria Portaria { get; set; } = null!;

    [Column("IdFornecedorEmpresa")]
    public int? IdFornecedorEmpresa { get; set; }

    [Column("NomeFornecedor")]
    [StringLength(200)]
    [Required]
    public string NomeFornecedor { get; set; } = string.Empty;

    [Column("CNPJFornecedor")]
    [StringLength(18)]
    public string? CNPJFornecedor { get; set; }

    [Column("DataHoraChegada")]
    [Required]
    public DateTime DataHoraChegada { get; set; }

    [Column("DataHoraInicio")]
    public DateTime? DataHoraInicio { get; set; }

    [Column("DataHoraFim")]
    public DateTime? DataHoraFim { get; set; }

    [Column("NumeroNotaFiscal")]
    [StringLength(50)]
    public string? NumeroNotaFiscal { get; set; }

    [Column("SerieNotaFiscal")]
    [StringLength(10)]
    public string? SerieNotaFiscal { get; set; }

    [Column("DataNotaFiscal")]
    public DateTime? DataNotaFiscal { get; set; }

    [Column("ValorNotaFiscal", TypeName = "decimal(18,2)")]
    public decimal? ValorNotaFiscal { get; set; }

    [Column("ChaveNFe")]
    [StringLength(50)]
    public string? ChaveNFe { get; set; }

    [Column("PesoBruto", TypeName = "decimal(18,2)")]
    public decimal? PesoBruto { get; set; }

    [Column("PesoTara", TypeName = "decimal(18,2)")]
    public decimal? PesoTara { get; set; }

    [Column("PesoLiquido", TypeName = "decimal(18,2)")]
    public decimal? PesoLiquido { get; set; }

    [Column("QuantidadeVolumes")]
    public int? QuantidadeVolumes { get; set; }

    [Column("QuantidadeVolumesConferidos")]
    public int? QuantidadeVolumesConferidos { get; set; }

    [Column("NumeroLacreOriginal")]
    [StringLength(50)]
    public string? NumeroLacreOriginal { get; set; }

    [Column("LacreViolado")]
    public bool? LacreViolado { get; set; }

    [Column("Status")]
    [StringLength(20)]
    [Required]
    public string Status { get; set; } = "AGUARDANDO";

    [Column("IdMotivoRecusa")]
    public int? IdMotivoRecusa { get; set; }

    [ForeignKey(nameof(IdMotivoRecusa))]
    public virtual MotivoRecusa? MotivoRecusa { get; set; }

    [Column("ObservacaoRecusa")]
    [StringLength(500)]
    public string? ObservacaoRecusa { get; set; }

    [Column("ConferidoPor")]
    public Guid? ConferidoPor { get; set; }

    [Column("NomeConferente")]
    [StringLength(150)]
    public string? NomeConferente { get; set; }

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

    [InverseProperty(nameof(RecebimentoCargaProduto.RecebimentoCarga))]
    public virtual ICollection<RecebimentoCargaProduto> Produtos { get; set; } = new List<RecebimentoCargaProduto>();
}
