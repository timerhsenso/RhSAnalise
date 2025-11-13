// src/Modules/GestaoDePessoas/Application/DTOs/MunicipioDto.cs

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;

/// <summary>
/// DTO para município (usado em Create, Update e Read).
/// </summary>
public sealed record MunicipioDto
{
    /// <summary>ID único (null para criação).</summary>
    public Guid? Id { get; init; }

    [Required(ErrorMessage = "O código do município é obrigatório")]
    [StringLength(5, MinimumLength = 5, ErrorMessage = "O código deve ter exatamente 5 dígitos")]
    [RegularExpression(@"^\d{5}$", ErrorMessage = "O código deve conter apenas números")]
    public string CodigoMunicipio { get; init; } = string.Empty;

    [Required(ErrorMessage = "A sigla do estado é obrigatória")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "A UF deve ter exatamente 2 caracteres")]
    [RegularExpression(@"^[A-Z]{2}$", ErrorMessage = "A UF deve conter apenas letras maiúsculas")]
    public string SiglaEstado { get; init; } = string.Empty;

    [Required(ErrorMessage = "O nome do município é obrigatório")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 60 caracteres")]
    public string NomeMunicipio { get; init; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "O código IBGE deve ser maior que zero")]
    public int? CodigoIBGE { get; init; }
}