using RhSensoERP.Generators;
using RhSensoERP.Shared.Core.Primitives;
using System.ComponentModel.DataAnnotations;

namespace RhSensoERP.Identity.Domain.Entities;

/// <summary>
/// Entidade que representa um sistema no ERP (tabela tsistema)
/// </summary>
[GenerateCrud(
    TableName = "tsistema",           // Nome da tabela no banco
    DisplayName = "Sistema"         // Nome para mensagens genéricas
)]
public class Sistema 
{
    // =========================================================================
    // CHAVE PRIMÁRIA
    // =========================================================================

    /// <summary>
    /// Código do sistema (PK) - VARCHAR(10)
    /// </summary>
    [Key]
    [Required(ErrorMessage = "O código do sistema é obrigatório.")]
    [StringLength(10, MinimumLength = 1, ErrorMessage = "O código deve ter entre 1 e 10 caracteres.")]
    [FieldDisplayName("Código do Sistema")]     // ← Nome de exibição
    [ColumnName("cdsistema")]                   // ← Nome da coluna no banco
    public string CdSistema { get; set; } = string.Empty;

    // =========================================================================
    // CAMPOS OBRIGATÓRIOS
    // =========================================================================

    /// <summary>
    /// Descrição do sistema - VARCHAR(60)
    /// </summary>
    [Required(ErrorMessage = "A descrição do sistema é obrigatória.")]
    [StringLength(60, MinimumLength = 3, ErrorMessage = "A descrição deve ter entre 3 e 60 caracteres.")]
    [FieldDisplayName("Descrição")]             // ← Nome de exibição
    [ColumnName("dcsistema")]                   // ← Nome da coluna no banco
    public string DcSistema { get; set; } = string.Empty;

    // =========================================================================
    // CAMPOS OPCIONAIS (sem [Required])
    // =========================================================================

    /// <summary>
    /// Sistema ativo - BIT - default 1 (true)
    /// </summary>
    [FieldDisplayName("Ativo")]
    [ColumnName("ativo")]
    public bool Ativo { get; set; } = true;

    // =========================================================================
    // NAVEGAÇÃO (ignoradas automaticamente pelo generator)
    // =========================================================================

    /// <summary>
    /// Funções do sistema
    /// </summary>
    public virtual ICollection<Funcao> Funcoes { get; set; } = new List<Funcao>();
}