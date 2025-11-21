// src/Web/Models/Base/BaseListViewModel.cs

namespace RhSensoERP.Web.Models.Base;

/// <summary>
/// ViewModel base para páginas de listagem com DataTables.
/// </summary>
public abstract class BaseListViewModel
{
    /// <summary>
    /// Título da página.
    /// </summary>
    public string PageTitle { get; set; } = string.Empty;

    /// <summary>
    /// Subtítulo da página.
    /// </summary>
    public string? PageSubtitle { get; set; }

    /// <summary>
    /// Ícone da página (Font Awesome).
    /// </summary>
    public string PageIcon { get; set; } = "fas fa-list";

    /// <summary>
    /// Nome do controller.
    /// </summary>
    public string ControllerName { get; set; } = string.Empty;

    /// <summary>
    /// Nome da action para listagem (DataTables Ajax).
    /// </summary>
    public string ListActionName { get; set; } = "List";

    /// <summary>
    /// Nome da action para criar.
    /// </summary>
    public string CreateActionName { get; set; } = "Create";

    /// <summary>
    /// Nome da action para editar.
    /// </summary>
    public string EditActionName { get; set; } = "Edit";

    /// <summary>
    /// Nome da action para visualizar.
    /// </summary>
    public string ViewActionName { get; set; } = "Details";

    /// <summary>
    /// Nome da action para excluir.
    /// </summary>
    public string DeleteActionName { get; set; } = "Delete";

    /// <summary>
    /// Nome da action para excluir múltiplos.
    /// </summary>
    public string DeleteMultipleActionName { get; set; } = "DeleteMultiple";

    /// <summary>
    /// Código da função para controle de permissões.
    /// </summary>
    public string? CdFuncao { get; set; }

    /// <summary>
    /// Permissões do usuário para esta funcionalidade (ex: "IAEC").
    /// I = Incluir, A = Alterar, E = Excluir, C = Consultar
    /// </summary>
    public string? UserPermissions { get; set; }

    /// <summary>
    /// Indica se o usuário pode incluir registros.
    /// </summary>
    public bool CanCreate => UserPermissions?.Contains('I') == true;

    /// <summary>
    /// Indica se o usuário pode alterar registros.
    /// </summary>
    public bool CanEdit => UserPermissions?.Contains('A') == true;

    /// <summary>
    /// Indica se o usuário pode excluir registros.
    /// </summary>
    public bool CanDelete => UserPermissions?.Contains('E') == true;

    /// <summary>
    /// Indica se o usuário pode consultar registros.
    /// </summary>
    public bool CanView => UserPermissions?.Contains('C') == true;

    /// <summary>
    /// Indica se deve exibir o botão de criar.
    /// </summary>
    public bool ShowCreateButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir o botão de excluir múltiplos.
    /// </summary>
    public bool ShowDeleteMultipleButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir o botão de atualizar.
    /// </summary>
    public bool ShowRefreshButton { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir os botões de exportação.
    /// </summary>
    public bool ShowExportButtons { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir a coluna de seleção (checkbox).
    /// </summary>
    public bool ShowSelectColumn { get; set; } = true;

    /// <summary>
    /// Indica se deve exibir a coluna de ações.
    /// </summary>
    public bool ShowActionsColumn { get; set; } = true;

    /// <summary>
    /// Configurações adicionais do DataTables (JSON).
    /// </summary>
    public string? AdditionalDataTablesConfig { get; set; }
}
