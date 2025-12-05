using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

[GenerateCrud(TableName = "SGC_ConfiguracaoEmpresa", DisplayName = "Configuração", CdSistema = "CAP", CdFuncao = "CAP_CFG_EMPRESA", IsLegacyTable = false, GenerateApiController = true)]
[Table("SGC_ConfiguracaoEmpresa")]
public class ConfiguracaoEmpresa
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("IdSaas")]
    public Guid? IdSaas { get; set; }

    [Column("TempoMaximoPermanencia")]
    public int? TempoMaximoPermanencia { get; set; }

    [Column("AlertaTempoExcedido")]
    public bool AlertaTempoExcedido { get; set; } = true;

    [Column("ExigeFotoVisitante")]
    public bool ExigeFotoVisitante { get; set; }

    [Column("ExigeBiometria")]
    public bool ExigeBiometria { get; set; }

    [Column("PermiteAcessoSemIntegracao")]
    public bool PermiteAcessoSemIntegracao { get; set; }

    [Column("DiasValidadeDocumento")]
    public int? DiasValidadeDocumento { get; set; }

    [Column("DiasAntecedenciaVencimento")]
    public int? DiasAntecedenciaVencimento { get; set; }

    [Column("NotificarVencimento")]
    public bool NotificarVencimento { get; set; } = true;

    [Column("EmailsNotificacao")]
    [StringLength(500)]
    public string? EmailsNotificacao { get; set; }

    [Column("HorarioInicioExpediente")]
    [StringLength(5)]
    public string? HorarioInicioExpediente { get; set; }

    [Column("HorarioFimExpediente")]
    [StringLength(5)]
    public string? HorarioFimExpediente { get; set; }

    [Column("PermiteAcessoForaExpediente")]
    public bool PermiteAcessoForaExpediente { get; set; }

    [Column("ExigeAutorizacaoForaExpediente")]
    public bool ExigeAutorizacaoForaExpediente { get; set; } = true;

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
