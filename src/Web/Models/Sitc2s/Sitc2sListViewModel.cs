// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: Sitc2
// Data: 2025-11-28 21:48:47
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Sitc2s;

/// <summary>
/// ViewModel para listagem de Situação de Frequência.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Sitc2sListViewModel : BaseListViewModel
{
    public Sitc2sListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Sitc2s", "Situação de Frequência");
        
        // Configurações específicas
        PageTitle = "Situação de Frequência";
        PageIcon = "fas fa-list";
        CdFuncao = "CPT_FM_SITC2";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Sitc2Dto> Items { get; set; } = new();
}
