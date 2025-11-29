// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: TabTaux1
// Data: 2025-11-28 23:45:52
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.TpTabelas;

/// <summary>
/// Request para atualização de Tipo de Tabela.
/// Compatível com backend: UpdateTabTaux1Request
/// </summary>
public class UpdateTabTaux1Request
{
    /// <summary>
    /// Descrição
    /// </summary>
    [Display(Name = "Descrição")]
    [Required(ErrorMessage = "Descrição é obrigatório")]
    [StringLength(60, ErrorMessage = "Descrição deve ter no máximo {1} caracteres")]
    public string DcTabela { get; set; } = string.Empty;
}
