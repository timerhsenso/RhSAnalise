// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoTreinamento
// Data: 2025-12-02 19:47:20
// =============================================================================

namespace RhSensoERP.Web.Models.SGC_TipoTreinamentos;

/// <summary>
/// DTO de leitura para Tipo Treinamento.
/// Compatível com backend: RhSensoERP.Modules.GestaoDePessoas.Application.DTOs.SGC_TipoTreinamentos.SGC_TipoTreinamentoDto
/// </summary>
public class SGC_TipoTreinamentoDto
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Saas
    /// </summary>
    public Guid? IdSaas { get; set; }

    /// <summary>
    /// Codigo
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Descricao
    /// </summary>
    public string Descricao { get; set; } = string.Empty;

    /// <summary>
    /// Carga Horaria Horas
    /// </summary>
    public decimal? CargaHorariaHoras { get; set; }

    /// <summary>
    /// Validade Em Meses
    /// </summary>
    public int? ValidadeEmMeses { get; set; }

    /// <summary>
    /// Ativo
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Aud Createdat
    /// </summary>
    public DateTime AudCreatedat { get; set; }

    /// <summary>
    /// Aud Updatedat
    /// </summary>
    public DateTime? AudUpdatedat { get; set; }

    /// <summary>
    /// Aud Idusuariocada Stro
    /// </summary>
    public Guid? AudIdusuariocadaStro { get; set; }

    /// <summary>
    /// Aud Idusuarioatualizacao
    /// </summary>
    public Guid? AudIdusuarioatualizacao { get; set; }
}
