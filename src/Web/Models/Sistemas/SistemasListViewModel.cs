// =============================================================================
// ARQUIVO GERADO POR RhSensoERP.CrudTool
// Entity: Sistema
// Data: 2025-11-27 01:25:15
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// ViewModel para listagem de Sistema.
/// </summary>
public class SistemasListViewModel : BaseListViewModel
{
    public SistemasListViewModel()
    {
        InitializeDefaults("Sistemas", "Sistemas");
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables).
    /// </summary>
    public List<SistemaDto> Items { get; set; } = new();
}
