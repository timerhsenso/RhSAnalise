// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool v2.0
// Entity: TabTaux1
// Data: 2025-11-28 23:45:52
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.TpTabelas;

/// <summary>
/// ViewModel para listagem de Tipo de Tabela.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class TpTabelasListViewModel : BaseListViewModel
{
    public TpTabelasListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("TpTabelas", "Tipo de Tabela");
        
        // Configurações específicas
        PageTitle = "Tipo de Tabela";
        PageIcon = "fas fa-list";
        CdFuncao = "SEG_FM_TSISTEMA";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<TabTaux1Dto> Items { get; set; } = new();
}
