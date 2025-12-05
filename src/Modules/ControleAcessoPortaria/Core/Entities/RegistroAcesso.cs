using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_RegistroAcesso", DisplayName = "Registro de Acesso", CdSistema = "CAP", CdFuncao = "CAP_MOV_REGISTROACESSO", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_RegistroAcesso")]
public class RegistroAcesso
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("NumeroProtocolo")]
    [StringLength(30)]
    [Required]
    public string NumeroProtocolo { get; set; } = string.Empty;

    [Column("TipoAcesso")]
    [StringLength(30)]
    [Required]
    public string TipoAcesso { get; set; } = string.Empty;

    [Column("IdPortaria")]
    [Required]
    public int IdPortaria { get; set; }

    [ForeignKey(nameof(IdPortaria))]
    public virtual Portaria Portaria { get; set; } = null!;

    [Column("IdFornecedorColaborador")]
    public int? IdFornecedorColaborador { get; set; }

    [Column("IdVisitante")]
    public int? IdVisitante { get; set; }

    [ForeignKey(nameof(IdVisitante))]
    public virtual Visitante? Visitante { get; set; }

    [Column("NomePessoa")]
    [StringLength(150)]
    [Required]
    public string NomePessoa { get; set; } = string.Empty;

    [Column("CPFPessoa")]
    [StringLength(14)]
    public string? CPFPessoa { get; set; }

    [Column("EmpresaPessoa")]
    [StringLength(200)]
    public string? EmpresaPessoa { get; set; }

    [Column("CargoPessoa")]
    [StringLength(100)]
    public string? CargoPessoa { get; set; }

    [Column("FotoPessoaUrl")]
    [StringLength(500)]
    public string? FotoPessoaUrl { get; set; }

    [Column("IdMotivoAcesso")]
    [Required]
    public int IdMotivoAcesso { get; set; }

    [ForeignKey(nameof(IdMotivoAcesso))]
    public virtual MotivoAcesso MotivoAcesso { get; set; } = null!;

    [Column("IdStatusAcesso")]
    [Required]
    public int IdStatusAcesso { get; set; }

    [ForeignKey(nameof(IdStatusAcesso))]
    public virtual StatusAcesso StatusAcesso { get; set; } = null!;

    [Column("IdAreaDestino")]
    public int? IdAreaDestino { get; set; }

    [ForeignKey(nameof(IdAreaDestino))]
    public virtual AreaAcesso? AreaDestino { get; set; }

    [Column("SetorDestino")]
    [StringLength(100)]
    public string? SetorDestino { get; set; }

    [Column("ResponsavelDestino")]
    [StringLength(150)]
    public string? ResponsavelDestino { get; set; }

    [Column("TelefoneResponsavel")]
    [StringLength(20)]
    public string? TelefoneResponsavel { get; set; }

    [Column("DataHoraEntrada")]
    [Required]
    public DateTime DataHoraEntrada { get; set; }

    [Column("DataHoraSaida")]
    public DateTime? DataHoraSaida { get; set; }

    [Column("PrevisaoSaida")]
    public DateTime? PrevisaoSaida { get; set; }

    [Column("IdCrachaProvisorio")]
    public int? IdCrachaProvisorio { get; set; }

    [ForeignKey(nameof(IdCrachaProvisorio))]
    public virtual CrachaProvisorio? CrachaProvisorio { get; set; }

    [Column("NumeroCracha")]
    [StringLength(20)]
    public string? NumeroCracha { get; set; }

    [Column("IdMotivoRecusa")]
    public int? IdMotivoRecusa { get; set; }

    [ForeignKey(nameof(IdMotivoRecusa))]
    public virtual MotivoRecusa? MotivoRecusa { get; set; }

    [Column("ObservacaoRecusa")]
    [StringLength(500)]
    public string? ObservacaoRecusa { get; set; }

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

    [InverseProperty(nameof(RegistroAcessoVeiculo.RegistroAcesso))]
    public virtual ICollection<RegistroAcessoVeiculo> Veiculos { get; set; } = new List<RegistroAcessoVeiculo>();

    [InverseProperty(nameof(ChecklistExecucao.RegistroAcesso))]
    public virtual ICollection<ChecklistExecucao> Checklists { get; set; } = new List<ChecklistExecucao>();
}
