// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoTreinamento
// Data: 2025-12-02 19:47:20
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.SGC_TipoTreinamentos;

/// <summary>
/// ViewModel para listagem de Tipo Treinamento.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class SGC_TipoTreinamentosListViewModel : BaseListViewModel
{
    public SGC_TipoTreinamentosListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("SGC_TipoTreinamentos", "Tipo Treinamento");
        
        // Configurações específicas
        PageTitle = "Tipo Treinamento";
        PageIcon = "fas fa-list";
        CdFuncao = "RHU_FM_TAUX1";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<SGC_TipoTreinamentoDto> Items { get; set; } = new();
}
