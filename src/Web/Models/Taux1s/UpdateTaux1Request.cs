// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Taux1
// Data: 2025-12-01 01:19:10
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Taux1s;

/// <summary>
/// Request para atualização de Tabela Auxiliar.
/// Compatível com backend: UpdateTaux1Request
/// </summary>
public class UpdateTaux1Request
{
    /// <summary>
    /// Dctabela
    /// </summary>
    [Display(Name = "Dctabela")]
    [Required(ErrorMessage = "Dctabela é obrigatório")]
    [StringLength(60, ErrorMessage = "Dctabela deve ter no máximo {1} caracteres")]
    public string Dctabela { get; set; } = string.Empty;
}
