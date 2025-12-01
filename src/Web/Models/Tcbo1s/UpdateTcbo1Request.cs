// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Tcbo1
// Data: 2025-12-01 01:28:26
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Tcbo1s;

/// <summary>
/// Request para atualização de Tabela de Ocupação.
/// Compatível com backend: UpdateTcbo1Request
/// </summary>
public class UpdateTcbo1Request
{
    /// <summary>
    /// Dccbo
    /// </summary>
    [Display(Name = "Dccbo")]
    [Required(ErrorMessage = "Dccbo é obrigatório")]
    [StringLength(80, ErrorMessage = "Dccbo deve ter no máximo {1} caracteres")]
    public string Dccbo { get; set; } = string.Empty;

    /// <summary>
    /// Si Nonimo
    /// </summary>
    [Display(Name = "Si Nonimo")]
    [StringLength(4000, ErrorMessage = "Si Nonimo deve ter no máximo {1} caracteres")]
    public string SiNonimo { get; set; } = string.Empty;

    /// <summary>
    /// Id
    /// </summary>
    [Display(Name = "Id")]
    [Required(ErrorMessage = "Id é obrigatório")]
    public Guid Id { get; set; }
}
