// =============================================================================
// RHSENSOERP WEB - MENU DISCOVERY SERVICE
// =============================================================================
// Arquivo: src/Web/Services/Menu/MenuDiscoveryService.cs
// Descrição: Descobre automaticamente Controllers marcados com [MenuItem]
// Versão: 2.0 - Com logs de debug para diagnóstico de permissões
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
/// <remarks>
/// <para><b>Funcionamento:</b></para>
/// <list type="number">
///   <item>Na inicialização, escaneia todos os Controllers com [MenuItem]</item>
///   <item>Ao montar o menu, verifica permissão 'C' (Consulta) para cada item</item>
///   <item>Itens sem permissão são ocultados do menu</item>
///   <item>Resultado é cacheado por 5 minutos por usuário</item>
/// </list>
/// 
/// <para><b>Diagnóstico:</b></para>
/// <para>Para debugar problemas de menu, configure o log level para Debug:</para>
/// <code>
/// "Logging": {
///   "LogLevel": {
///     "RhSensoERP.Web.Services.Menu": "Debug"
///   }
/// }
/// </code>
/// </remarks>
public interface IMenuDiscoveryService
{
    /// <summary>
    /// Obtém todos os módulos com seus itens de menu.
    /// </summary>
    /// <param name="username">Username para verificar permissões. Se null, usa o usuário do HttpContext.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de módulos com seus itens de menu.</returns>
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

    // =========================================================================
    // CACHE ESTÁTICO
    // =========================================================================

    /// <summary>
    /// Cache estático - carregado uma vez na inicialização (todos os itens possíveis).
    /// Contém TODOS os Controllers com [MenuItem], independente de permissão.
    /// </summary>
    private static readonly Lazy<IReadOnlyList<MenuItemInfo>> _allMenuItems = new(DiscoverMenuItems);

    /// <summary>
    /// Cache por usuário (menu já filtrado por permissão) + TTL.
    /// Chave: username, Valor: menu filtrado + timestamp.
    /// </summary>
    private static readonly ConcurrentDictionary<string, CachedMenuEntry> _userMenuCache = new();

    /// <summary>
    /// Duração do cache por usuário.
    /// Após este tempo, as permissões são verificadas novamente.
    /// </summary>
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    // =========================================================================
    // CONSTRUTOR
    // =========================================================================

    public MenuDiscoveryService(
        IUserPermissionsCacheService permissionsService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<MenuDiscoveryService> logger)
    {
        _permissionsService = permissionsService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    // =========================================================================
    // DESCOBERTA DE CONTROLLERS (EXECUTADO UMA VEZ)
    // =========================================================================

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

    // =========================================================================
    // OBTENÇÃO DO MENU (COM VERIFICAÇÃO DE PERMISSÕES)
    // =========================================================================

    public async Task<IReadOnlyList<MenuModuleViewModel>> GetMenuAsync(
        string? username = null,
        CancellationToken ct = default)
    {
        // =====================================================================
        // PASSO 1: Obter username
        // =====================================================================
        username ??= _httpContextAccessor.HttpContext?.User?.Identity?.Name;

        // DEBUG: Log do username obtido
        _logger.LogDebug(
            "[MENU] Iniciando GetMenuAsync | Username recebido: '{UsernameParam}' | " +
            "Username do HttpContext: '{UsernameContext}' | " +
            "IsAuthenticated: {IsAuth}",
            username,
            _httpContextAccessor.HttpContext?.User?.Identity?.Name,
            _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated);

        if (string.IsNullOrWhiteSpace(username))
        {
            _logger.LogWarning(
                "[MENU] ⚠️ Username vazio ou nulo! Menu será retornado vazio. " +
                "Verifique se o usuário está autenticado.");
            return Array.Empty<MenuModuleViewModel>();
        }

        // =====================================================================
        // PASSO 2: Verificar cache
        // =====================================================================
        if (_userMenuCache.TryGetValue(username, out var cachedEntry))
        {
            var age = DateTimeOffset.UtcNow - cachedEntry.CreatedAt;
            if (age <= CacheDuration)
            {
                _logger.LogDebug(
                    "[MENU] Cache HIT para '{User}' | Idade: {Age:mm\\:ss} | " +
                    "Módulos: {Modules} | Itens: {Items}",
                    username,
                    age,
                    cachedEntry.Menu.Count,
                    cachedEntry.Menu.Sum(m => m.Items.Count));
                return cachedEntry.Menu;
            }

            // Expirado: remove para forçar recarga
            _userMenuCache.TryRemove(username, out _);
            _logger.LogDebug(
                "[MENU] Cache EXPIRADO para '{User}' | Idade: {Age:mm\\:ss} | Recarregando...",
                username,
                age);
        }

        // =====================================================================
        // PASSO 3: Carregar todos os itens descobertos
        // =====================================================================
        var allItems = _allMenuItems.Value;

        _logger.LogDebug(
            "[MENU] Total de itens descobertos (todos os Controllers com [MenuItem]): {Count}",
            allItems.Count);

        // DEBUG: Lista todos os itens descobertos
        foreach (var item in allItems)
        {
            _logger.LogDebug(
                "[MENU] Item descoberto: {DisplayName} | Controller: {Controller} | " +
                "CdFuncao: {CdFuncao} | Módulo: {Module}",
                item.DisplayName,
                item.ControllerName,
                item.CdFuncao ?? "(sem permissão definida)",
                item.Module);
        }

        var modules = new List<MenuModuleViewModel>();

        // =====================================================================
        // PASSO 4: Agrupar por módulo e verificar permissões
        // =====================================================================
        var groupedByModule = allItems.GroupBy(i => i.Module);

        foreach (var group in groupedByModule.OrderBy(g => GetModuleOrder(g.Key)))
        {
            var moduleInfo = GetModuleInfo(group.Key);
            var moduleItems = new List<MenuItemViewModel>();

            _logger.LogDebug(
                "[MENU] Processando módulo: {Module} ({DisplayName}) | Itens no módulo: {Count}",
                group.Key,
                moduleInfo.DisplayName,
                group.Count());

            foreach (var item in group.OrderBy(i => i.Order).ThenBy(i => i.DisplayName))
            {
                // =============================================================
                // VERIFICAÇÃO DE PERMISSÃO
                // =============================================================
                var hasPermission = true;

                if (!string.IsNullOrEmpty(item.CdFuncao))
                {
                    // Verifica permissão 'C' (Consulta) para exibir no menu
                    hasPermission = await _permissionsService.HasPermissionAsync(
                        username,
                        item.CdFuncao!,
                        'C', // <-- Tipo de permissão: Consulta
                        ct);

                    // DEBUG: Log detalhado da verificação de permissão
                    _logger.LogDebug(
                        "[MENU] Verificando permissão | Item: {DisplayName} | " +
                        "CdFuncao: {CdFuncao} | Usuário: {User} | " +
                        "Tipo: 'C' (Consulta) | Resultado: {HasPermission}",
                        item.DisplayName,
                        item.CdFuncao,
                        username,
                        hasPermission ? "✅ PERMITIDO" : "❌ NEGADO");
                }
                else
                {
                    _logger.LogDebug(
                        "[MENU] Item sem CdFuncao (sempre visível): {DisplayName}",
                        item.DisplayName);
                }

                // =============================================================
                // DECISÃO: MOSTRAR OU OCULTAR
                // =============================================================
                if (!hasPermission && !item.ComingSoon)
                {
                    _logger.LogDebug(
                        "[MENU] ❌ Item OCULTO do menu: {DisplayName} | " +
                        "Motivo: Usuário '{User}' não tem permissão 'C' em '{CdFuncao}'",
                        item.DisplayName,
                        username,
                        item.CdFuncao);
                    continue;
                }

                // Item será adicionado ao menu
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

                _logger.LogDebug(
                    "[MENU] ✅ Item ADICIONADO ao menu: {DisplayName}",
                    item.DisplayName);
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

                _logger.LogDebug(
                    "[MENU] Módulo adicionado: {Module} | Itens visíveis: {Count}",
                    moduleInfo.DisplayName,
                    moduleItems.Count);
            }
            else
            {
                _logger.LogDebug(
                    "[MENU] Módulo IGNORADO (sem itens visíveis): {Module}",
                    moduleInfo.DisplayName);
            }
        }

        // =====================================================================
        // PASSO 5: Armazenar em cache
        // =====================================================================
        var entry = new CachedMenuEntry
        {
            CreatedAt = DateTimeOffset.UtcNow,
            Menu = modules.AsReadOnly()
        };

        _userMenuCache[username] = entry;

        // =====================================================================
        // RESUMO FINAL
        // =====================================================================
        _logger.LogInformation(
            "[MENU] Menu montado para '{User}' | " +
            "Módulos: {Modules} | Itens totais: {Items} | " +
            "Cache atualizado (TTL: {TTL} min)",
            username,
            modules.Count,
            modules.Sum(m => m.Items.Count),
            CacheDuration.TotalMinutes);

        if (modules.Count == 0)
        {
            _logger.LogWarning(
                "[MENU] ⚠️ Menu VAZIO para '{User}'! Possíveis causas:\n" +
                "  1. Usuário não tem permissão 'C' (Consulta) nas funções\n" +
                "  2. Funções não existem na tabela tfunc1\n" +
                "  3. Permissões não cadastradas na tabela tperm1\n" +
                "  4. Nenhum Controller com [MenuItem] foi descoberto",
                username);
        }

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
        var count = _userMenuCache.Count;
        _userMenuCache.Clear();
        _logger.LogInformation(
            "[MENU] Cache invalidado | Entradas removidas: {Count}",
            count);
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