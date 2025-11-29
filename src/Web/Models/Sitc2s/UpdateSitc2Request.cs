// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: Sitc2
// Data: 2025-11-28 21:48:47
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Sitc2s;

/// <summary>
/// Request para atualização de Situação de Frequência.
/// Compatível com backend: UpdateSitc2Request
/// </summary>
public class UpdateSitc2Request
{
    /// <summary>
    /// Código Empresa
    /// </summary>
    [Display(Name = "Código Empresa")]
    [Required(ErrorMessage = "Código Empresa é obrigatório")]
    public int CdEmpresa { get; set; }

    /// <summary>
    /// Código Filial
    /// </summary>
    [Display(Name = "Código Filial")]
    [Required(ErrorMessage = "Código Filial é obrigatório")]
    public int CdFilial { get; set; }

    /// <summary>
    /// Matrícula
    /// </summary>
    [Display(Name = "Matrícula")]
    [Required(ErrorMessage = "Matrícula é obrigatório")]
    [StringLength(8, ErrorMessage = "Matrícula deve ter no máximo {1} caracteres")]
    public string NoMatric { get; set; } = string.Empty;

    /// <summary>
    /// Data Frequência
    /// </summary>
    [Display(Name = "Data Frequência")]
    [Required(ErrorMessage = "Data Frequência é obrigatório")]
    public DateTime DtFrequen { get; set; }

    /// <summary>
    /// Situação
    /// </summary>
    [Display(Name = "Situação")]
    [Required(ErrorMessage = "Situação é obrigatório")]
    public int FlSituacao { get; set; }

    /// <summary>
    /// Código Usuário
    /// </summary>
    [Display(Name = "Código Usuário")]
    [StringLength(20, ErrorMessage = "Código Usuário deve ter no máximo {1} caracteres")]
    public string? CdUsuario { get; set; }

    /// <summary>
    /// Data Última Movimentação
    /// </summary>
    [Display(Name = "Data Última Movimentação")]
    [Required(ErrorMessage = "Data Última Movimentação é obrigatório")]
    public DateTime DtUltMov { get; set; }

    /// <summary>
    /// Processado
    /// </summary>
    [Display(Name = "Processado")]
    [Required(ErrorMessage = "Processado é obrigatório")]
    public int FlProcessado { get; set; }

    /// <summary>
    /// Importado
    /// </summary>
    [Display(Name = "Importado")]
    [Required(ErrorMessage = "Importado é obrigatório")]
    public int FlImportado { get; set; }

    /// <summary>
    /// Data Importação
    /// </summary>
    [Display(Name = "Data Importação")]
    public DateTime? DtImportacao { get; set; }

    /// <summary>
    /// Data Processamento
    /// </summary>
    [Display(Name = "Data Processamento")]
    public DateTime? DtProcessamento { get; set; }

    /// <summary>
    /// Id Funcionário
    /// </summary>
    [Display(Name = "Id Funcionário")]
    public Guid? IdFuncionario { get; set; }
}
