using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Tabela 8 - Classificação Tributária (eSocial)
/// Utilizada no evento S-1000 (Empregador)
/// Tabela: tab8_esocial
/// </summary>
/// <remarks>
/// Empresas do módulo GestaoDePessoas referenciam esta tabela pelo código (ClassTrib).
/// Para listar empresas que usam esta classificação, usar IGestaoDePessoasLookupService.
/// </remarks>
[GenerateCrud(
    TableName = "tab8_esocial",
    DisplayName = "Tabela 8 - Classificação Tributária",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_TAB08",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tab8_esocial")]
public class EsocialTabela8
{
    [Key]
    [Column("tab8_codigo")]
    [StringLength(2)]
    [Display(Name = "Código")]
    public string Tab8Codigo { get; set; } = string.Empty;

    [Column("tab8_descricao")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Tab8Descricao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // ❌ SEM Collections de OUTRO módulo (GestaoDePessoas)
    // Empresa referencia esta tabela via código string (ClassTrib)
    // Para buscar empresas: IGestaoDePessoasLookupService.GetEmpresasByClassTrib()
    // ═══════════════════════════════════════════════════════════════════
}
