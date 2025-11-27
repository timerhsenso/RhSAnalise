// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// DTO para criação de Sistema.
/// </summary>
public class CreateSistemaDto
{
    /// <summary>
    /// Código
    /// </summary>
    [Display(Name = "Código")]
    [Required(ErrorMessage = "Código é obrigatório")]
    [StringLength(10, ErrorMessage = "Código deve ter no máximo 10 caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição deve ter no máximo 60 caracteres")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}
