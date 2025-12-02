// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoTreinamento
// Data: 2025-12-02 19:47:20
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.SGC_TipoTreinamentos;

/// <summary>
/// Request para atualização de Tipo Treinamento.
/// Compatível com backend: UpdateSGC_TipoTreinamentoRequest
/// </summary>
public class UpdateSGC_TipoTreinamentoRequest
{
    /// <summary>
    /// Saas
    /// </summary>
    [Display(Name = "Saas")]
    public Guid? IdSaas { get; set; }

    /// <summary>
    /// Codigo
    /// </summary>
    [Display(Name = "Codigo")]
    [Required(ErrorMessage = "Codigo é obrigatório")]
    [StringLength(20, ErrorMessage = "Codigo deve ter no máximo {1} caracteres")]
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao
    /// </summary>
    [Display(Name = "Descricao")]
    [Required(ErrorMessage = "Descricao é obrigatório")]
    [StringLength(150, ErrorMessage = "Descricao deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Carga Horaria Horas
    /// </summary>
    [Display(Name = "Carga Horaria Horas")]
    public decimal? CargaHorariaHoras { get; set; }

    /// <summary>
    /// Validade Em Meses
    /// </summary>
    [Display(Name = "Validade Em Meses")]
    public int? ValidadeEmMeses { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Aud Createdat
    /// </summary>
    [Display(Name = "Aud Createdat")]
    [Required(ErrorMessage = "Aud Createdat é obrigatório")]
    public DateTime AudCreatedat { get; set; }

    /// <summary>
    /// Aud Updatedat
    /// </summary>
    [Display(Name = "Aud Updatedat")]
    public DateTime? AudUpdatedat { get; set; }

    /// <summary>
    /// Aud Idusuariocada Stro
    /// </summary>
    [Display(Name = "Aud Idusuariocada Stro")]
    public Guid? AudIdusuariocadaStro { get; set; }

    /// <summary>
    /// Aud Idusuarioatualizacao
    /// </summary>
    [Display(Name = "Aud Idusuarioatualizacao")]
    public Guid? AudIdusuarioatualizacao { get; set; }
}
