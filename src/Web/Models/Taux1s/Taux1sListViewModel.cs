// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.0
// Entity: Taux1
// Data: 2025-12-01 23:06:17
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Taux1s;

/// <summary>
/// ViewModel para listagem de Tabela Auxiliar.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class Taux1sListViewModel : BaseListViewModel
{
    public Taux1sListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("Taux1s", "Tabela Auxiliar");
        
        // Configurações específicas
        PageTitle = "Tabela Auxiliar";
        PageIcon = "fas fa-list";
        CdFuncao = "RHU_FM_TAUX1";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<Taux1Dto> Items { get; set; } = new();
}
