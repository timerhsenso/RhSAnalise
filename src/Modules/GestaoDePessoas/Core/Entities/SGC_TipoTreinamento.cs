using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

/// <summary>
/// Tabela auxiliar (catálogo) de tipos de treinamento com carga horária e validade padrão. Base para controle de capacitação de funcionários e terceiros conforme NRs.
/// </summary>
[GenerateCrud(
    TableName = "sgc_tipotreinamento",
    DisplayName = "Tipo Treinamento",
    CdSistema = "RHU",
    CdFuncao = "RHU_FM_TAUX1",
    IsLegacyTable = true,
    GenerateApiController = true
)]
public class SGC_TipoTreinamento
{
    /// <summary>
    /// Identificador único do tipo de treinamento. Chave primária auto-incremento.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Identificador do tenant (SaaS). NULL para dados globais.
    /// </summary>
    public Guid? IdSaas { get; set; }

    /// <summary>
    /// Código único do treinamento (ex: NR06, NR10, NR20, NR33, NR35, INTEGRACAO, CIPA, BRIGADA). Máx. 20 caracteres.
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descrição completa (ex: NR-10 - Segurança em Instalações Elétricas, NR-35 - Trabalho em Altura).
    /// </summary>
    [Required]
    [StringLength(150)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Carga horária mínima em horas conforme norma. Ex: 40h para NR-10, 8h para NR-35. Decimal para permitir meias-horas.
    /// </summary>
    public decimal? CargaHorariaHoras { get; set; }

    /// <summary>
    /// Validade em meses conforme norma. Ex: 24 meses para NR-10, 12 meses para NR-33. NULL se não tiver validade.
    /// </summary>
    public int? ValidadeEmMeses { get; set; }

    /// <summary>
    /// Status ativo (1) ou inativo (0).
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data/hora UTC de criação.
    /// </summary>
    public DateTime Aud_CreatedAt { get; set; }

    /// <summary>
    /// Data/hora UTC da última atualização.
    /// </summary>
    public DateTime? Aud_UpdatedAt { get; set; }

    /// <summary>
    /// Usuário que criou (FK → tuse1.id).
    /// </summary>
    public Guid? Aud_IdUsuarioCadastro { get; set; }

    /// <summary>
    /// Usuário que atualizou (FK → tuse1.id).
    /// </summary>
    public Guid? Aud_IdUsuarioAtualizacao { get; set; }

    #region Navigation Properties

    /// <summary>
    /// Navegação para Tuse1 via Aud_IdUsuarioAtualizacao
    /// </summary>
    [ForeignKey(nameof(Aud_IdUsuarioAtualizacao))]
    public virtual Usuario? AudIdusuarioatualizacao { get; set; }

    /// <summary>
    /// Navegação para Tuse1 via Aud_IdUsuarioCadastro
    /// </summary>
    [ForeignKey(nameof(Aud_IdUsuarioCadastro))]
    public virtual Usuario? AudIdusuariocadastro2 { get; set; }

    #endregion
}
