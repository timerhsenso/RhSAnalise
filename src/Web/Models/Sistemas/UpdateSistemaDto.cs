// src/Web/Models/Sistemas/UpdateSistemaDto.cs

using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// DTO para atualização de Sistema.
/// </summary>
public sealed class UpdateSistemaDto
{
    /// <summary>
    /// Código do sistema (PK).
    /// **ADICIONADO PARA GARANTIR O MODEL BINDING CORRETO NA API**
    /// </summary>
    [Required(ErrorMessage = "O código do sistema é obrigatório")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O código deve ter entre 1 e 10 caracteres")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do sistema.
    /// </summary>
    [Required(ErrorMessage = "A descrição do sistema é obrigatória")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 100 caracteres")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Indica se o sistema está ativo.
    /// </summary>
    public bool Ativo { get; set; } = true;
}
