// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using System.Text.Json.Serialization;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// DTO de leitura para Sistema.
/// </summary>
public class SistemaDto
{
    /// <summary>
    /// Código
    /// </summary>
    [JsonPropertyName("cdSistema")]
    public string CdSistema { get; set; } = string.Empty;

    /// <summary>
    /// Descrição
    /// </summary>
    [JsonPropertyName("dcSistema")]
    public string DcSistema { get; set; } = string.Empty;

    /// <summary>
    /// Ativo
    /// </summary>
    [JsonPropertyName("ativo")]
    public bool Ativo { get; set; } = true;
}
