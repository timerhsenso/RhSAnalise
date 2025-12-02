// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Taux1
// Data: 2025-12-01 23:06:17
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Taux1s;

/// <summary>
/// Request para criação de Tabela Auxiliar.
/// Compatível com backend: CreateTaux1Request
/// </summary>
public class CreateTaux1Request
{
    /// <summary>
    /// Dctabela
    /// </summary>
    [Display(Name = "Dctabela")]
    [Required(ErrorMessage = "Dctabela é obrigatório")]
    [StringLength(60, ErrorMessage = "Dctabela deve ter no máximo {1} caracteres")]
    public string Dctabela { get; set; } = string.Empty;
}
