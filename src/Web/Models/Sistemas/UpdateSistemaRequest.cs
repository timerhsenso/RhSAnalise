// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: Sistema
// Data: 2025-11-28 21:37:44
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// Request para atualização de Sistema.
/// Compatível com backend: UpdateSistemaRequest
/// </summary>
public class UpdateSistemaRequest
{
    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
