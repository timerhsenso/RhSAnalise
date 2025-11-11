using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Modules.GestaoDePessoas.Application.DTOs;

public class BancoDto
{
    public string CodigoBanco { get; set; } = default!;
    public string DescricaoBanco { get; set; } = default!;
    public int TotalAgencias { get; set; }
}

public class CreateBancoDto
{
    [Required(ErrorMessage = "Código do banco é obrigatório")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "Código deve ter 3 caracteres")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Código deve conter apenas números")]
    public string CodigoBanco { get; set; } = default!;

    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo 40 caracteres")]
    public string DescricaoBanco { get; set; } = default!;
}

public class UpdateBancoDto
{
    [Required(ErrorMessage = "Descrição é obrigatória")]
    [StringLength(40, ErrorMessage = "Descrição deve ter no máximo 40 caracteres")]
    public string DescricaoBanco { get; set; } = default!;
}