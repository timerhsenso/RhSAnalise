// =============================================================================
// ARQUIVO GERADO POR GeradorFullStack v3.1
// Entity: SGC_TipoFornecedor
// Data: 2025-12-02 15:55:54
// =============================================================================
using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.SGC_TipoFornecedores;

/// <summary>
/// ViewModel para listagem de Tipo Fornecedor.
/// Herda de BaseListViewModel que já contém permissões e configurações de DataTables.
/// </summary>
public class SGC_TipoFornecedoresListViewModel : BaseListViewModel
{
    public SGC_TipoFornecedoresListViewModel()
    {
        // Inicializa propriedades padrão
        InitializeDefaults("SGC_TipoFornecedores", "Tipo Fornecedor");
        
        // Configurações específicas
        PageTitle = "Tipo Fornecedor";
        PageIcon = "fas fa-list";
        CdFuncao = "RHU_FM_TAUX1";
    }

    /// <summary>
    /// Itens da listagem (para uso sem DataTables server-side).
    /// </summary>
    public List<SGC_TipoFornecedorDto> Items { get; set; } = new();
}
