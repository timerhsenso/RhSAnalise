// src/Web/Models/Bancos/BancoDto.cs

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Bancos;

/// <summary>
/// DTO completo de Banco.
/// </summary>
public sealed class BancoDto
{
    /// <summary>
    /// Código do banco (chave primária).
    /// </summary>
    public string CodigoBanco { get; set; } = string.Empty;

    /// <summary>
    /// Nome do banco.
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o banco está ativo.
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Data de criação.
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Data de alteração.
    /// </summary>
    public DateTime? DataAlteracao { get; set; }
}

/// <summary>
/// DTO para criação de Banco.
/// </summary>
public sealed class CreateBancoDto
{
    /// <summary>
    /// Código do banco (chave primária).
    /// </summary>
    [Required(ErrorMessage = "O código do banco é obrigatório")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "O código deve ter exatamente 3 caracteres")]
    [Display(Name = "Código do Banco")]
    public string CodigoBanco { get; set; } = string.Empty;

    /// <summary>
    /// Nome do banco.
    /// </summary>
    [Required(ErrorMessage = "O nome do banco é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    [Display(Name = "Nome do Banco")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o banco está ativo.
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; } = true;
}

/// <summary>
/// DTO para atualização de Banco.
/// </summary>
public sealed class UpdateBancoDto
{
    /// <summary>
    /// Nome do banco.
    /// </summary>
    [Required(ErrorMessage = "O nome do banco é obrigatório")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
    [Display(Name = "Nome do Banco")]
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o banco está ativo.
    /// </summary>
    [Display(Name = "Ativo")]
    public bool Ativo { get; set; }
}
