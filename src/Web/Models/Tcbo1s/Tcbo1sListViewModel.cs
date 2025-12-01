// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Tcbo1
// Data: 2025-12-01 01:28:26
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Tcbo1s;

/// <summary>
/// ViewModel para listagem de Tabela de Ocupação.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Tcbo1sListViewModel : BaseListViewModel
{
    public Tcbo1sListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Tcbo1s", "Tabela de Ocupação");
        
        // Configurações específicas
        PageTitle = "Tabela de Ocupação";
        PageIcon = "fas fa-list";
        CdFuncao = "RHU_FM_TCBO1";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Tcbo1Dto> Items { get; set; } = new();
}
