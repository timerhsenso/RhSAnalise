namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

/// <summary>
/// Situações/tipos auxiliares por categoria (Taux1).
/// Tabela legada: taux2 | Chave composta: (cdtptabela, cdsituacao)
/// </summary>
public class Taux2
{
    /// <summary>
    /// Código do tipo de tabela (PK parte 1, FK -> Taux1).
    /// Exemplos: "GI", "SR", "EC", "RA", "SE"
    /// </summary>
    public string CdTpTabela { get; set; } = default!;

    /// <summary>
    /// Código da situação (PK parte 2).
    /// Exemplos: "01", "02", "03", etc.
    /// </summary>
    public string CdSituacao { get; set; } = default!;

    /// <summary>
    /// Descrição da situação.
    /// Exemplos: "Analfabeto", "Ensino Fundamental", "Ensino Médio"
    /// </summary>
    public string DcSituacao { get; set; } = default!;

    /// <summary>
    /// Número de ordem para ordenação.
    /// </summary>
    public int? NoOrdem { get; set; }

    /// <summary>
    /// Flag de ativo/inativo (S/N).
    /// </summary>
    public string? FlAtivoAux { get; set; }

    // ==================== NAVEGAÇÃO ====================
    /// <summary>
    /// Tipo de tabela ao qual esta situação pertence.
    /// </summary>
    public virtual Taux1 TipoTabela { get; set; } = default!;
}