// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoFornecedor
// Data: 2025-12-02 15:55:54
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.SGC_TipoFornecedores;

/// <summary>
/// Request para atualização de Tipo Fornecedor.
/// Compatível com backend: UpdateSGC_TipoFornecedorRequest
/// </summary>
public class UpdateSGC_TipoFornecedorRequest
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
    [StringLength(100, ErrorMessage = "Descricao deve ter no máximo {1} caracteres")]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Icone
    /// </summary>
    [Display(Name = "Icone")]
    [StringLength(50, ErrorMessage = "Icone deve ter no máximo {1} caracteres")]
    public string Icone { get; set; } = string.Empty;

    /// <summary>
    /// Cor Hex
    /// </summary>
    [Display(Name = "Cor Hex")]
    [StringLength(7, ErrorMessage = "Cor Hex deve ter no máximo {1} caracteres")]
    public string CorHex { get; set; } = string.Empty;

    /// <summary>
    /// Ordem
    /// </summary>
    [Display(Name = "Ordem")]
    [Required(ErrorMessage = "Ordem é obrigatório")]
    public int Ordem { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    [Display(Name = "Ativo")]
    [Required(ErrorMessage = "Ativo é obrigatório")]
    public bool Ativo { get; set; }

    /// <summary>
    /// Data Cadastro
    /// </summary>
    [Display(Name = "Data Cadastro")]
    [Required(ErrorMessage = "Data Cadastro é obrigatório")]
    public DateTime DataCadastro { get; set; }

    /// <summary>
    /// Data Atualizacao
    /// </summary>
    [Display(Name = "Data Atualizacao")]
    public DateTime? DataAtualizacao { get; set; }

    /// <summary>
    /// Usuario Cadastro
    /// </summary>
    [Display(Name = "Usuario Cadastro")]
    public Guid? IdUsuarioCadastro { get; set; }

    /// <summary>
    /// Usuario Atualizacao
    /// </summary>
    [Display(Name = "Usuario Atualizacao")]
    public Guid? IdUsuarioAtualizacao { get; set; }
}
