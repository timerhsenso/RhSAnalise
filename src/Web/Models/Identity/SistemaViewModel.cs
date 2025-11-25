// src/Web/Models/Identity/SistemaViewModel.cs

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Identity;

/// <summary>
/// ViewModel para exibição de Sistema na listagem.
/// </summary>
public sealed class SistemaViewModel
{
    /// <summary>
    /// Código do sistema (PK).
    /// </summary>
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema.
    /// </summary>
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o sistema está ativo.
    /// </summary>
    public bool Ativo { get; set; }
}

/// <summary>
/// ViewModel para criação de Sistema.
/// </summary>
public sealed class CreateSistemaViewModel
{
    /// <summary>
    /// Código do sistema (PK).
    /// </summary>
    [Required(ErrorMessage = "O código do sistema é obrigatório.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O código deve ter entre 1 e 10 caracteres.")]
    [Display(Name = "Código")]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "O código deve conter apenas letras maiúsculas e números.")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema.
    /// </summary>
    [Required(ErrorMessage = "A descrição do sistema é obrigatória.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 100 caracteres.")]
    [Display(Name = "Descrição")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o sistema está ativo.
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}

/// <summary>
/// ViewModel para edição de Sistema.
/// </summary>
public sealed class UpdateSistemaViewModel
{
    /// <summary>
    /// Código do sistema (PK) - somente leitura na edição.
    /// </summary>
    [Display(Name = "Código")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema.
    /// </summary>
    [Required(ErrorMessage = "A descrição do sistema é obrigatória.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 100 caracteres.")]
    [Display(Name = "Descrição")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o sistema está ativo.
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}
