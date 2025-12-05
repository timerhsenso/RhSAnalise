using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_Portaria", DisplayName = "Portaria", CdSistema = "CAP", CdFuncao = "CAP_CAD_PORTARIA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_Portaria")]
public class Portaria
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("Codigo")]
    [StringLength(20)]
    [Required]
    public string Codigo { get; set; } = string.Empty;

    [Column("Nome")]
    [StringLength(100)]
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Column("Descricao")]
    [StringLength(200)]
    public string? Descricao { get; set; }

    [Column("PermiteAcessoPedestre")]
    public bool PermiteAcessoPedestre { get; set; } = true;

    [Column("PermiteAcessoVeiculo")]
    public bool PermiteAcessoVeiculo { get; set; } = true;

    [Column("PermiteAcessoCarga")]
    public bool PermiteAcessoCarga { get; set; }

    [Column("PossuiBalanca")]
    public bool PossuiBalanca { get; set; }

    [Column("PossuiCatraca")]
    public bool PossuiCatraca { get; set; }

    [Column("HorarioFuncionamentoInicio")]
    [StringLength(5)]
    public string? HorarioFuncionamentoInicio { get; set; }

    [Column("HorarioFuncionamentoFim")]
    [StringLength(5)]
    public string? HorarioFuncionamentoFim { get; set; }

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

    [InverseProperty(nameof(RegistroAcesso.Portaria))]
    public virtual ICollection<RegistroAcesso> RegistrosAcesso { get; set; } = new List<RegistroAcesso>();

    [InverseProperty(nameof(CrachaProvisorio.Portaria))]
    public virtual ICollection<CrachaProvisorio> CrachasProvisiorios { get; set; } = new List<CrachaProvisorio>();
}
