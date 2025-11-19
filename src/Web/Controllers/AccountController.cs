using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhSensoERP.Web.Models.Account;
using RhSensoERP.Web.Services;

namespace RhSensoERP.Web.Controllers;

/// <summary>
/// Controller para autenticação de usuários.
/// </summary>
public sealed class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthApiService authApiService, ILogger<AccountController> logger)
    {
        _authApiService = authApiService;
        _logger = logger;
    }

    /// <summary>
    /// Exibe a página de login.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        // Se já estiver autenticado, redireciona
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToLocal(returnUrl);
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    /// <summary>
    /// Processa o login do usuário.
    /// </summary>
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
            // Chama a API para autenticar
            var authResponse = await _authApiService.LoginAsync(model, ct);

            if (authResponse == null)
            {
                ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
                _logger.LogWarning("Tentativa de login falhou: {CdUsuario}", model.CdUsuario);
                return View(model);
            }

            // Cria as claims do usuário
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, authResponse.User?.Id.ToString() ?? Guid.Empty.ToString()),
                new(ClaimTypes.Name, authResponse.User?.DcUsuario ?? model.CdUsuario),
                new("cdusuario", authResponse.User?.CdUsuario ?? model.CdUsuario),
                new("dcusuario", authResponse.User?.DcUsuario ?? model.CdUsuario),
                new("AccessToken", authResponse.AccessToken),
                new("RefreshToken", authResponse.RefreshToken),
                new("TokenExpiry", authResponse.ExpiresAt.ToString("O"))
            };

            // Adiciona claims opcionais
            if (authResponse.User?.NoMatric != null)
            {
                claims.Add(new Claim("nomatric", authResponse.User.NoMatric));
            }

            if (authResponse.User?.CdEmpresa != null)
            {
                claims.Add(new Claim("cdempresa", authResponse.User.CdEmpresa.ToString()!));
            }

            if (authResponse.User?.CdFilial != null)
            {
                claims.Add(new Claim("cdfilial", authResponse.User.CdFilial.ToString()!));
            }

            if (authResponse.User?.TenantId != null)
            {
                claims.Add(new Claim("tenantid", authResponse.User.TenantId.ToString()!));
            }

            if (!string.IsNullOrWhiteSpace(authResponse.User?.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, authResponse.User.Email));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(8),
                AllowRefresh = true
            };

            // Faz o sign in com cookie
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

    /// <summary>
    /// Realiza o logout do usuário.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = User.FindFirstValue("RefreshToken");

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authApiService.LogoutAsync(refreshToken);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Logout realizado: {CdUsuario}", User.FindFirstValue("cdusuario"));

            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer logout");
            return RedirectToAction(nameof(Login));
        }
    }

    /// <summary>
    /// Página de acesso negado.
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    /// <summary>
    /// Dashboard com informações e permissões do usuário.
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        try
        {
            var accessToken = User.FindFirstValue("AccessToken");
            var cdUsuario = User.FindFirstValue("cdusuario");

            if (string.IsNullOrWhiteSpace(accessToken) || string.IsNullOrWhiteSpace(cdUsuario))
            {
                _logger.LogWarning("Token ou código de usuário não encontrado nas claims");
                return RedirectToAction(nameof(Login));
            }

            // Busca informações do usuário
            var userInfo = await _authApiService.GetCurrentUserAsync(accessToken, ct);

            if (userInfo == null)
            {
                _logger.LogWarning("Não foi possível obter informações do usuário: {CdUsuario}", cdUsuario);
                return RedirectToAction(nameof(Login));
            }

            // Busca permissões do usuário
            var permissions = await _authApiService.GetUserPermissionsAsync(cdUsuario, null, ct);

            var viewModel = new DashboardViewModel
            {
                UserInfo = userInfo,
                Permissions = permissions ?? new UserPermissionsViewModel(),
                HasPermissionsError = permissions == null,
                ErrorMessage = permissions == null ? "Não foi possível carregar as permissões do usuário." : null
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar dashboard do usuário");
            return RedirectToAction("Error", "Home");
        }
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}