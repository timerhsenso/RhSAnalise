// src/Modules/GestaoDePessoas/Application/DTOs/SindicatoDto.cs

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;

/// <summary>
/// DTO para leitura de sindicato.
/// </summary>
public sealed record SindicatoDto
{
    /// <summary>ID único.</summary>
    public Guid Id { get; init; }

    /// <summary>Código do sindicato (2 caracteres).</summary>
    public string CodigoSindicato { get; init; } = string.Empty;

    /// <summary>Descrição/nome do sindicato.</summary>
    public string DescricaoSindicato { get; init; } = string.Empty;

    /// <summary>Endereço do sindicato.</summary>
    public string? Endereco { get; init; }

    /// <summary>CNPJ do sindicato.</summary>
    public string? CNPJ { get; init; }

    /// <summary>Código da entidade.</summary>
    public string? CodigoEntidade { get; init; }

    /// <summary>Data base (mês de negociação).</summary>
    public string? DataBase { get; init; }

    /// <summary>Flag tipo de sindicato.</summary>
    public int? FlagTipo { get; init; }

    /// <summary>Código da tabela base.</summary>
    public string? CodigoTabelaBase { get; init; }
}

/// <summary>
/// DTO para criação de sindicato.
/// </summary>
public sealed record CreateSindicatoDto
{
    [Required(ErrorMessage = "O código do sindicato é obrigatório")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "O código deve ter exatamente 2 caracteres")]
    public string CodigoSindicato { get; init; } = string.Empty;

    [Required(ErrorMessage = "A descrição do sindicato é obrigatória")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 40 caracteres")]
    public string DescricaoSindicato { get; init; } = string.Empty;

    [StringLength(30, ErrorMessage = "O endereço deve ter no máximo 30 caracteres")]
    public string? Endereco { get; init; }

    [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 caracteres")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter apenas números")]
    public string? CNPJ { get; init; }

    [StringLength(20, ErrorMessage = "O código da entidade deve ter no máximo 20 caracteres")]
    public string? CodigoEntidade { get; init; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A data base deve ter 2 caracteres (mês)")]
    [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Data base deve ser um mês válido (01-12)")]
    public string? DataBase { get; init; }

    [Range(0, 9, ErrorMessage = "O flag tipo deve estar entre 0 e 9")]
    public int? FlagTipo { get; init; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "O código da tabela base deve ter 3 caracteres")]
    public string? CodigoTabelaBase { get; init; }
}

/// <summary>
/// DTO para atualização de sindicato.
/// </summary>
public sealed record UpdateSindicatoDto
{
    [Required(ErrorMessage = "A descrição do sindicato é obrigatória")]
    [StringLength(40, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 40 caracteres")]
    public string DescricaoSindicato { get; init; } = string.Empty;

    [StringLength(30, ErrorMessage = "O endereço deve ter no máximo 30 caracteres")]
    public string? Endereco { get; init; }

    [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve ter 14 caracteres")]
    [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter apenas números")]
    public string? CNPJ { get; init; }

    [StringLength(20, ErrorMessage = "O código da entidade deve ter no máximo 20 caracteres")]
    public string? CodigoEntidade { get; init; }

    [StringLength(2, MinimumLength = 2, ErrorMessage = "A data base deve ter 2 caracteres (mês)")]
    [RegularExpression(@"^(0[1-9]|1[0-2])$", ErrorMessage = "Data base deve ser um mês válido (01-12)")]
    public string? DataBase { get; init; }

    [Range(0, 9, ErrorMessage = "O flag tipo deve estar entre 0 e 9")]
    public int? FlagTipo { get; init; }

    [StringLength(3, MinimumLength = 3, ErrorMessage = "O código da tabela base deve ter 3 caracteres")]
    public string? CodigoTabelaBase { get; init; }
}