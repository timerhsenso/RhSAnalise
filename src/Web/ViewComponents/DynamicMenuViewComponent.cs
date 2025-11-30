// =============================================================================
// RHSENSOERP WEB - DYNAMIC MENU VIEW COMPONENT
// =============================================================================
// Arquivo: src/Web/ViewComponents/DynamicMenuViewComponent.cs
// Descrição: Renderiza o menu dinamicamente baseado nos Controllers descobertos
// =============================================================================

using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Services.Menu;

namespace RhSensoERP.Web.ViewComponents;

/// <summary>
/// ViewComponent que renderiza o menu dinâmico.
/// Uso na View: @await Component.InvokeAsync("DynamicMenu")
/// </summary>
public class DynamicMenuViewComponent : ViewComponent
{
    private readonly IMenuDiscoveryService _menuService;

    public DynamicMenuViewComponent(IMenuDiscoveryService menuService)
    {
        _menuService = menuService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string? viewName = null)
    {
        var menu = await _menuService.GetMenuAsync();

        // Se não houver itens (ex: usuário não autenticado ou sem permissão),
        // evita quebrar o layout.
        if (menu == null || menu.Count == 0)
        {
            return Content("<!-- Menu dinâmico vazio para este usuário -->");
        }

        return View(viewName ?? "Default", menu);
    }
}
