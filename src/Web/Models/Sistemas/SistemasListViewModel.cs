// ============================================================================
// SISTEMAS LIST VIEW MODEL (COM CONTROLE DE PERMISSÕES)
// ============================================================================
// Arquivo: Models/Sistemas/SistemasListViewModel.cs
// Versão: 3.0 (Compatível com BaseListViewModel)
//
// ViewModel para listagem de Sistemas.
// Herda de BaseListViewModel que já possui as propriedades de permissões.
//
// ============================================================================

using RhSensoERP.Web.Models.Base;

namespace RhSensoERP.Web.Models.Sistemas;

/// <summary>
/// ViewModel para listagem de Sistemas.
/// </summary>
public sealed class SistemasListViewModel : BaseListViewModel
{
    /// <summary>
    /// Construtor padrão.
    /// Inicializa propriedades específicas de Sistemas.
    /// </summary>
    public SistemasListViewModel()
    {
        // Informações da página
        PageTitle = "Gestão de Sistemas";
        PageSubtitle = "Cadastro e gerenciamento de sistemas do ERP";
        PageIcon = "fas fa-server";

        // Configuração do controller
        ControllerName = "Sistemas";

        // Código da função para controle de permissões
        CdFuncao = "SEG_FM_TSISTEMA";

        // Configurações de exportação
        ExportEnabled = true;
        ExportExcel = true;
        ExportPdf = true;
        ExportCsv = true;
        ExportPrint = true;
        ExportFilename = "Sistemas";

        // Configurações DataTables
        PageLength = 10;
        LengthMenuOptions = new[] { 10, 25, 50, 100 };
        DefaultOrderColumn = 1; // Ordena por código
        DefaultOrderDirection = "asc";

        // Elementos de UI
        ShowCreateButton = true;
        ShowDeleteMultipleButton = true;
        ShowRefreshButton = true;
        ShowExportButtons = true;
        ShowSelectColumn = true;
        ShowActionsColumn = true;
        ShowSearchBox = true;

        // Mensagens customizadas (opcional)
        EmptyTableMessage = "Nenhum sistema cadastrado. Clique em 'Novo' para adicionar.";
    }

    // =========================================================================
    // NOTA: As propriedades de permissões já estão na classe BaseListViewModel:
    // - UserPermissions (string "IAEC")
    // - CanCreate (bool calculado)
    // - CanEdit (bool calculado)
    // - CanDelete (bool calculado)
    // - CanView (bool calculado)
    // =========================================================================
}
