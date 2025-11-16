using System.Collections.Generic;

namespace RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

/// <summary>
/// Tipos de tabelas auxiliares (Legado: taux1).
/// Define categorias de situações/classificações usadas em Taux2.
/// Exemplos: GI (Grau de Instrução), SR (Situação na Receita), etc.
/// </summary>
public class Taux1
{
    /// <summary>
    /// Código do tipo de tabela (PK).
    /// Exemplos: "GI", "SR", "EC", "RA", "SE"
    /// </summary>
    public string CdTpTabela { get; set; } = default!;

    /// <summary>
    /// Descrição do tipo de tabela.
    /// Exemplos: "Grau de Instrução", "Situação na Receita"
    /// </summary>
    public string DcTabela { get; set; } = default!;

    // ==================== NAVEGAÇÃO ====================
    /// <summary>
    /// Situações/classificações vinculadas a este tipo de tabela.
    /// </summary>
    public virtual ICollection<Taux2> Situacoes { get; set; } = new HashSet<Taux2>();
}