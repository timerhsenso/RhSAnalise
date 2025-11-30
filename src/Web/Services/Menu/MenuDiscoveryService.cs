// =============================================================================
// RHSENSOERP WEB - MENU DISCOVERY SERVICE
// =============================================================================
// Arquivo: src/Web/Services/Menu/MenuDiscoveryService.cs
// Descrição: Descobre automaticamente Controllers marcados com [MenuItem]
// =============================================================================

using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Attributes;
using RhSensoERP.Web.Services.Permissions;

namespace RhSensoERP.Web.Services.Menu;

/// <summary>
/// Serviço que descobre e gerencia itens de menu automaticamente.
/// </summary>
public interface IMenuDiscoveryService
{
    /// <summary>
    /// Obtém todos os módulos com seus itens de menu.
    /// </summary>
    Task<IReadOnlyList<MenuModuleViewModel>> GetMenuAsync(string? username = null, CancellationToken ct = default);

    /// <summary>
    /// Obtém itens de um módulo específico.
    /// </summary>
    Task<MenuModuleViewModel?> GetModuleMenuAsync(MenuModule module, string? username = null, CancellationToken ct = default);

    /// <summary>
    /// Força recarga do cache de menu (útil após deploy ou alteração de permissões).
    /// </summary>
    void InvalidateCache();
}

/// <summary>
/// Implementação do serviço de descoberta de menu.
/// </summary>
public class MenuDiscoveryService : IMenuDiscoveryService
{
    private readonly IUserPermissionsCacheService _permissionsService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<MenuDiscoveryService> _logger;

    // Cache estático - carregado uma vez na inicialização (todos os itens possíveis)
    private static readonly Lazy<IReadOnlyList<MenuItemInfo>> _allMenuItems = new(DiscoverMenuItems);

    // Cache por usuário (menu já filtrado por permissão) + TTL
    private static readonly ConcurrentDictionary<string, CachedMenuEntry> _userMenuCache = new();

    // Duração do cache por usuário (pode ajustar conforme necessidade)
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public MenuDiscoveryService(
        IUserPermissionsCacheService permissionsService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MenuDiscoveryService> logger)
    {
        _permissionsService = permissionsService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Descobre todos os Controllers marcados com [MenuItem].
    /// Executado uma única vez (Lazy) e em TODOS os assemblies carregados.
    /// </summary>
    private static IReadOnlyList<MenuItemInfo> DiscoverMenuItems()
    {
        var items = new List<MenuItemInfo>();

        // Pega todos os assemblies carregados (exceto dinâmicos)
        var assemblies = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(a => !a.IsDynamic)
            .ToArray();

        foreach (var assembly in assemblies)
        {
            Type[] types;

            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            }

            var controllers = types
                .Where(t => t is not null
                            && t.IsClass
                            && !t.IsAbstract
                            && typeof(Controller).IsAssignableFrom(t)
                            && t.GetCustomAttribute<MenuItemAttribute>() != null)
                .ToList();

            if (controllers.Count == 0)
            {
                continue;
            }

            foreach (var controller in controllers)
            {
                var attr = controller.GetCustomAttribute<MenuItemAttribute>()!;

                // Ignora itens marcados como Hidden
                if (attr.Hidden)
                {
                    continue;
                }

                // Nome do controller sem sufixo "Controller"
                var controllerName = controller.Name;
                if (controllerName.EndsWith("Controller", StringComparison.Ordinal))
                {
                    controllerName = controllerName[..^10];
                }

                // Descobre área: prioridade = atributo MenuItem.Area, depois [Area] no controller
                var areaAttr = controller.GetCustomAttribute<AreaAttribute>();
                var areaName = attr.Area ?? areaAttr?.RouteValue;

                items.Add(new MenuItemInfo
                {
                    ControllerName = controllerName,
                    ControllerType = controller,
                    Module = attr.Module,
                    DisplayName = attr.DisplayName ?? FormatDisplayName(controllerName),
                    Icon = attr.Icon,
                    Order = attr.Order,
                    CdFuncao = attr.CdFuncao,
                    CdSistema = attr.CdSistema,
                    ComingSoon = attr.ComingSoon,
                    Badge = attr.Badge,
                    BadgeColor = attr.BadgeColor,
                    Description = attr.Description,
                    Action = attr.Action,
                    RouteValues = attr.RouteValues,
                    Area = areaName
                });
            }
        }

        var ordered = items
            .OrderBy(i => i.Module)
            .ThenBy(i => i.Order)
            .ThenBy(i => i.DisplayName)
            .ToList()
            .AsReadOnly();

        return ordered;
    }

    /// <summary>
    /// Formata nome do controller para exibição.
    /// Ex: "Tabtaux1s" -> "Tabtaux1s", "GruposUsuario" -> "Grupos Usuário"
    /// </summary>
    private static string FormatDisplayName(string controllerName)
    {
        // Adiciona espaço antes de letras maiúsculas
        var result = System.Text.RegularExpressions.Regex.Replace(
            controllerName,
            "([a-z])([A-Z])",
            "$1 $2");

        return result;
    }

    public async Task<IReadOnlyList<MenuModuleViewModel>> GetMenuAsync(
        string? username = null,
        CancellationToken ct = default)
    {
        // Obtém username do contexto se não informado
        username ??= _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning("Tentativa de obter menu sem usuário autenticado");
            return Array.Empty<MenuModuleViewModel>();
        }

        // Verifica cache por usuário com TTL
        if (_userMenuCache.TryGetValue(username, out var cachedEntry))
        {
            var age = DateTimeOffset.UtcNow - cachedEntry.CreatedAt;
            if (age <= CacheDuration)
            {
                return cachedEntry.Menu;
            }

            // Expirado: remove para forçar recarga
            _userMenuCache.TryRemove(username, out _);
        }

        var allItems = _allMenuItems.Value;
        var modules = new List<MenuModuleViewModel>();

        // Agrupa por módulo
        var groupedByModule = allItems.GroupBy(i => i.Module);

        foreach (var group in groupedByModule.OrderBy(g => GetModuleOrder(g.Key)))
        {
            var moduleInfo = GetModuleInfo(group.Key);
            var moduleItems = new List<MenuItemViewModel>();

            foreach (var item in group.OrderBy(i => i.Order).ThenBy(i => i.DisplayName))
            {
                // Verifica permissão se CdFuncao foi definido
                var hasPermission = true;
                if (!string.IsNullOrEmpty(item.CdFuncao))
                {
                    hasPermission = await _permissionsService.HasPermissionAsync(
                        username,
                        item.CdFuncao!,
                        'C',
                        ct);
                }

                // Se não tem permissão e não é "ComingSoon", não mostra
                if (!hasPermission && !item.ComingSoon)
                {
                    continue;
                }

                moduleItems.Add(new MenuItemViewModel
                {
                    Area = item.Area,
                    Controller = item.ControllerName,
                    Action = item.Action,
                    DisplayName = item.DisplayName,
                    Icon = item.Icon,
                    CdFuncao = item.CdFuncao,
                    ComingSoon = item.ComingSoon,
                    Badge = item.ComingSoon ? "Em breve" : item.Badge,
                    BadgeColor = item.ComingSoon ? "secondary" : item.BadgeColor,
                    Description = item.Description,
                    HasPermission = hasPermission && !item.ComingSoon
                });
            }

            // Só adiciona módulo se tiver itens visíveis
            if (moduleItems.Count > 0)
            {
                modules.Add(new MenuModuleViewModel
                {
                    Module = group.Key,
                    DisplayName = moduleInfo.DisplayName,
                    Icon = moduleInfo.Icon,
                    Order = moduleInfo.Order,
                    CdSistema = moduleInfo.CdSistema,
                    Items = moduleItems
                });
            }
        }

        // Armazena em cache para o usuário com horário de criação
        var entry = new CachedMenuEntry
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Menu = modules.AsReadOnly()
        };

        _userMenuCache[username] = entry;

        _logger.LogDebug(
            "Menu carregado para {User}: {Modules} módulos, {Items} itens",
            username,
            modules.Count,
            modules.Sum(m => m.Items.Count));

        return entry.Menu;
    }

    public async Task<MenuModuleViewModel?> GetModuleMenuAsync(
        MenuModule module,
        string? username = null,
        CancellationToken ct = default)
    {
        var menu = await GetMenuAsync(username, ct);
        return menu.FirstOrDefault(m => m.Module == module);
    }

    public void InvalidateCache()
    {
        _userMenuCache.Clear();
        _logger.LogInformation("Cache de menu invalidado");
    }

    #region Helpers

    private static (string DisplayName, string Icon, int Order, string CdSistema) GetModuleInfo(MenuModule module)
    {
        var field = typeof(MenuModule).GetField(module.ToString());
        var attr = field?.GetCustomAttribute<MenuModuleInfoAttribute>();

        return attr != null
            ? (attr.DisplayName, attr.Icon, attr.Order, attr.CdSistema)
            : (module.ToString(), "fas fa-folder", 99, "OUT");
    }

    private static int GetModuleOrder(MenuModule module)
    {
        return GetModuleInfo(module).Order;
    }

    #endregion
}

#region ViewModels e estruturas internas

/// <summary>
/// Informações internas de um item de menu (cache).
/// </summary>
internal class MenuItemInfo
{
    public string ControllerName { get; set; } = string.Empty;
    public Type ControllerType { get; set; } = null!;
    public MenuModule Module { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-circle";
    public int Order { get; set; }
    public string? CdFuncao { get; set; }
    public string? CdSistema { get; set; }
    public bool ComingSoon { get; set; }
    public string? Badge { get; set; }
    public string BadgeColor { get; set; } = "info";
    public string? Description { get; set; }
    public string Action { get; set; } = "Index";
    public string? RouteValues { get; set; }

    /// <summary>
    /// Nome da área MVC (ex: "SEG", "RHU").
    /// </summary>
    public string? Area { get; set; }
}

/// <summary>
/// Entrada de cache do menu por usuário, com TTL.
/// </summary>
internal sealed class CachedMenuEntry
{
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<MenuModuleViewModel> Menu { get; init; } = Array.Empty<MenuModuleViewModel>();
}

/// <summary>
/// ViewModel de um módulo do menu.
/// </summary>
public class MenuModuleViewModel
{
    public MenuModule Module { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Order { get; set; }
    public string CdSistema { get; set; } = string.Empty;
    public IReadOnlyList<MenuItemViewModel> Items { get; set; } = Array.Empty<MenuItemViewModel>();
}

/// <summary>
/// ViewModel de um item do menu.
/// </summary>
public class MenuItemViewModel
{
    /// <summary>
    /// Nome da área MVC (pode ser nulo para controllers sem área).
    /// </summary>
    public string? Area { get; set; }

    public string Controller { get; set; } = string.Empty;
    public string Action { get; set; } = "Index";
    public string DisplayName { get; set; } = string.Empty;
    public string Icon { get; set; } = "fas fa-circle";
    public string? CdFuncao { get; set; }
    public bool ComingSoon { get; set; }
    public string? Badge { get; set; }
    public string BadgeColor { get; set; } = "info";
    public string? Description { get; set; }
    public bool HasPermission { get; set; } = true;

    /// <summary>
    /// URL completa do item, já considerando área (se houver).
    /// Ex: "/SEG/Usuarios/Index" ou "/Usuarios/Index".
    /// </summary>
    public string Url
    {
        get
        {
            var areaPrefix = string.IsNullOrWhiteSpace(Area) ? string.Empty : $"/{Area}";
            return $"{areaPrefix}/{Controller}/{Action}";
        }
    }
}

#endregion
