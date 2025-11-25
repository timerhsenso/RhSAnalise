// ============================================================================
// SISTEMAS LIST VIEW MODEL
// ============================================================================
// Arquivo: Models/Sistemas/SistemasListViewModel.cs
//
// ViewModel para listagem de Sistemas.
// Herda de BaseListViewModel e configura propriedades específicas.
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
        CdFuncao = "SIS001";

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
}
