using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RhSensoERP.Shared.Core.Attributes;

namespace RhSensoERP.Modules.Esocial.Core.Entities;

/// <summary>
/// Tabela 21 - Natureza Jurídica (eSocial)
/// Utilizada no evento S-1000 (Empregador)
/// Tabela: tab21_esocial
/// </summary>
/// <remarks>
/// Empresas do módulo GestaoDePessoas referenciam esta tabela pelo código (NatJuridica).
/// Para listar empresas que usam esta natureza jurídica, usar IGestaoDePessoasLookupService.
/// </remarks>
[GenerateCrud(
    TableName = "tab21_esocial",
    DisplayName = "Tabela 21 - Natureza Jurídica",
    CdSistema = "RHU",
    CdFuncao = "ESO_FM_TAB21",
    IsLegacyTable = true,
    GenerateApiController = true
)]
[Table("tab21_esocial")]
public class EsocialTabela21
{
    [Key]
    [Column("tab21_codigo")]
    [StringLength(4)]
    [Display(Name = "Código")]
    public string Tab21Codigo { get; set; } = string.Empty;

    [Column("tab21_descricao")]
    [StringLength(255)]
    [Display(Name = "Descrição")]
    public string Tab21Descricao { get; set; } = string.Empty;

    // ═══════════════════════════════════════════════════════════════════
    // ❌ SEM Collections de OUTRO módulo (GestaoDePessoas)
    // Empresa referencia esta tabela via código string (NatJuridica)
    // Para buscar empresas: IGestaoDePessoasLookupService.GetEmpresasByNatJuridica()
    // ═══════════════════════════════════════════════════════════════════
}
