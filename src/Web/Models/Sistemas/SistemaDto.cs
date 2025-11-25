// src/Web/Models/Sistemas/SistemaDto.cs

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// DTO para representação de Sistema.
/// </summary>
public sealed class SistemaDto
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
    public bool Ativo { get; set; } = true;

    /// <summary>
    /// Data de criação (se disponível na API).
    /// </summary>
    public DateTime? DataCriacao { get; set; }
}
