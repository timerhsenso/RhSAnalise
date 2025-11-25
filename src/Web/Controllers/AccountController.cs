// =============================================================================
// RHSENSOERP WEB - ACCOUNT CONTROLLER (ATUALIZADO COM CACHE DE PERMISSÕES)
// =============================================================================
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Extensions;
using RhSensoERP.Web.Models.Account;
using RhSensoERP.Web.Services;
using RhSensoERP.Web.Services.Permissions; // ✅ Adicionado using para o serviço de cache

namespace RhSensoERP.Web.Controllers;

public sealed class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly IUserPermissionsCacheService _permissionsCache; // ✅ Injetado o serviço de cache
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAuthApiService authApiService,
        IUserPermissionsCacheService permissionsCache, // ✅ Novo parâmetro
        ILogger<AccountController> logger)
    {
        _authApiService = authApiService;
        _permissionsCache = permissionsCache;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }
        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        ViewData["ReturnUrl"] = model.ReturnUrl;
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var authResponse = await _authApiService.LoginAsync(model, ct);
            if (authResponse == null || authResponse.User == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                return View(model);
            }

            // ============================================================================
            // ✅ NOVO: BUSCA E ARMAZENA AS PERMISSÕES NO CACHE
            // ============================================================================
            var userPermissions = await _authApiService.GetUserPermissionsAsync(authResponse.User.CdUsuario, null, ct);
            if (userPermissions != null)
            {
                // Armazena as permissões no cache com expiração alinhada ao token
                var tokenLifetime = authResponse.ExpiresAt - DateTime.UtcNow;
                _permissionsCache.Set(authResponse.User.CdUsuario, userPermissions, tokenLifetime);
                _logger.LogInformation("Permissões do usuário {CdUsuario} armazenadas no cache.", authResponse.User.CdUsuario);
            }
            else
            {
                _logger.LogWarning("Não foi possível obter as permissões para o usuário {CdUsuario}.", authResponse.User.CdUsuario);
            }
            // ============================================================================

            // Cria as claims MÍNIMAS para o cookie
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authResponse.User.Id.ToString()),
                new(ClaimTypes.Name, authResponse.User.DcUsuario),
                new("cdusuario", authResponse.User.CdUsuario),
                // Outras claims essenciais (NÃO incluir permissões aqui)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : authResponse.ExpiresAt,
                AllowRefresh = true
            };

            // Armazena os tokens para serem usados pelo HttpClient
            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken { Name = "access_token", Value = authResponse.AccessToken },
                new AuthenticationToken { Name = "refresh_token", Value = authResponse.RefreshToken },
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            _logger.LogInformation("Login bem-sucedido: {CdUsuario}", model.CdUsuario);
            return RedirectToLocal(model.ReturnUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar login: {CdUsuario}", model.CdUsuario);
            ModelState.AddModelError(string.Empty, "Erro ao processar login. Tente novamente.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var cdUsuario = User.GetCdUsuario();
        if (!string.IsNullOrEmpty(cdUsuario))
        {
            // ✅ NOVO: Remove as permissões do cache no logout
            _permissionsCache.Remove(cdUsuario);
            _logger.LogInformation("Permissões do usuário {CdUsuario} removidas do cache.", cdUsuario);
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Usuário deslogado com sucesso.");
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home");
    }
}
