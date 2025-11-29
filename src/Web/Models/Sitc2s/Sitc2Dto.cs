// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: Sitc2
// Data: 2025-11-28 21:48:47
// =============================================================================

namespace RhSensoERP.Web.Models.Sitc2s;

/// <summary>
/// DTO de leitura para Situação de Frequência.
/// Compatível com backend: RhSensoERP.Modules.ControleDePonto.Application.DTOs.Sitc2s.Sitc2Dto
/// </summary>
public class Sitc2Dto
{
    /// <summary>
    /// ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Código Empresa
    /// </summary>
    public int CdEmpresa { get; set; }

    /// <summary>
    /// Código Filial
    /// </summary>
    public int CdFilial { get; set; }

    /// <summary>
    /// Matrícula
    /// </summary>
    public string NoMatric { get; set; } = string.Empty;

    /// <summary>
    /// Data Frequência
    /// </summary>
    public DateTime DtFrequen { get; set; }

    /// <summary>
    /// Situação
    /// </summary>
    public int FlSituacao { get; set; }

    /// <summary>
    /// Código Usuário
    /// </summary>
    public string? CdUsuario { get; set; }

    /// <summary>
    /// Data Última Movimentação
    /// </summary>
    public DateTime DtUltMov { get; set; }

    /// <summary>
    /// Processado
    /// </summary>
    public int FlProcessado { get; set; }

    /// <summary>
    /// Importado
    /// </summary>
    public int FlImportado { get; set; }

    /// <summary>
    /// Data Importação
    /// </summary>
    public DateTime? DtImportacao { get; set; }

    /// <summary>
    /// Data Processamento
    /// </summary>
    public DateTime? DtProcessamento { get; set; }

    /// <summary>
    /// Id Funcionário
    /// </summary>
    public Guid? IdFuncionario { get; set; }
}
