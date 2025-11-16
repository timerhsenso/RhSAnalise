//C:\Users\eduardo\source\repos\RhSAnalise\src\API\Controllers\Identity
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RhSensoERP.Identity.Application.DTOs.Auth;
using RhSensoERP.Identity.Application.Features.Auth.Commands;
using RhSensoERP.Identity.Application.Features.Auth.Queries;
using System.Security.Claims;

namespace RhSensoERP.API.Controllers.Identity;

/// <summary>
/// Controller para autenticação e gestão de tokens.
/// </summary>
[ApiController]
[Route("api/identity/auth")]
[ApiExplorerSettings(GroupName = "Identity")]
public sealed class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Autentica um usuário e retorna tokens JWT.
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Tokens de acesso e refresh</returns>
    /// <response code="200">Login bem-sucedido</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Credenciais inválidas ou conta bloqueada</response>
    /// <response code="429">Limite de requisições excedido</response>
    /// <response code="504">Timeout na requisição</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")] 
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var ipAddress = GetIpAddress();
        var userAgent = GetUserAgent();

        _logger.LogInformation(
            "🔐 Tentativa de login: {CdUsuario} | IP: {IpAddress} | Strategy: {Strategy}",
            request.CdUsuario,
            ipAddress,
            request.AuthStrategy ?? "Default");

        try
        {
            var command = new LoginCommand(request, ipAddress, userAgent);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no login: {CdUsuario} | Erro: {ErrorCode} - {ErrorMessage}",
                    request.CdUsuario,
                    result.Error.Code,
                    result.Error.Message);

                return result.Error.Code switch
                {
                    "TIMEOUT" => StatusCode(504, new { error = result.Error.Code, message = result.Error.Message }),
                    "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Code, message = result.Error.Message }),
                    "INVALID_CREDENTIALS" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "USER_INACTIVE" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "ACCOUNT_LOCKED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "EMAIL_NOT_CONFIRMED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "2FA_REQUIRED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message, require2FA = true }),
                    _ => StatusCode(500, new { error = "LOGIN_ERROR", message = "Erro ao processar login." })
                };
            }

            _logger.LogInformation("✅ Login bem-sucedido: {CdUsuario}", request.CdUsuario);

            return Ok(result.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning(
                "⏱️ Timeout/Cancelamento na requisição de login: {CdUsuario} | IP: {IpAddress}",
                request.CdUsuario,
                ipAddress);

            return StatusCode(504, new
            {
                error = "TIMEOUT",
                message = "A requisição foi cancelada ou excedeu o tempo limite."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "💥 Erro inesperado no login: {CdUsuario} | IP: {IpAddress}",
                request.CdUsuario,
                ipAddress);

            return StatusCode(500, new
            {
                error = "INTERNAL_ERROR",
                message = "Erro interno ao processar login."
            });
        }
    }

    /// <summary>
    /// Renova tokens usando refresh token.
    /// </summary>
    /// <param name="request">Access token expirado e refresh token válido</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Novos tokens de acesso e refresh</returns>
    /// <response code="200">Tokens renovados com sucesso</response>
    /// <response code="400">Dados de entrada inválidos</response>
    /// <response code="401">Refresh token inválido ou expirado</response>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [EnableRateLimiting("refresh")] // ✅ ADICIONAR
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken ct)
    {
        var ipAddress = GetIpAddress();

        _logger.LogDebug("🔄 Tentativa de refresh token | IP: {IpAddress}", ipAddress);

        try
        {
            var command = new RefreshTokenCommand(request, ipAddress);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no refresh token | Erro: {ErrorCode} - {ErrorMessage}",
                    result.Error.Code,
                    result.Error.Message);

                return result.Error.Code switch
                {
                    "INVALID_REFRESH_TOKEN" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "USER_NOT_FOUND" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "ACCOUNT_LOCKED" => Unauthorized(new { error = result.Error.Code, message = result.Error.Message }),
                    "VALIDATION_ERROR" => BadRequest(new { error = result.Error.Code, message = result.Error.Message }),
                    _ => StatusCode(500, new { error = "REFRESH_ERROR", message = "Erro ao renovar tokens." })
                };
            }

            _logger.LogInformation("✅ Tokens renovados com sucesso");

            return Ok(result.Value);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("⏱️ Timeout na renovação de tokens | IP: {IpAddress}", ipAddress);
            return StatusCode(504, new { error = "TIMEOUT", message = "A requisição foi cancelada." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro inesperado no refresh token | IP: {IpAddress}", ipAddress);
            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao renovar tokens." });
        }
    }

    /// <summary>
    /// Logout do usuário (revoga refresh tokens).
    /// </summary>
    /// <param name="request">Parâmetros de logout</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Confirmação de logout</returns>
    /// <response code="200">Logout realizado com sucesso</response>
    /// <response code="401">Não autorizado</response>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest request, CancellationToken ct)
    {
        // ✅ FIX: Usar claim customizado "cdusuario" em vez de ClaimTypes.NameIdentifier
        var cdUsuario = User.FindFirstValue("cdusuario");

        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return Unauthorized(new { error = "INVALID_TOKEN", message = "Token inválido." });
        }

        _logger.LogInformation("🚪 Logout iniciado: {CdUsuario}", cdUsuario);

        try
        {
            var command = new LogoutCommand(cdUsuario, request);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning(
                    "❌ Falha no logout: {CdUsuario} | Erro: {ErrorCode} - {ErrorMessage}",
                    cdUsuario,
                    result.Error.Code,
                    result.Error.Message);

                return BadRequest(new { error = result.Error.Code, message = result.Error.Message });
            }

            _logger.LogInformation("✅ Logout realizado com sucesso: {CdUsuario}", cdUsuario);

            return Ok(new { message = "Logout realizado com sucesso." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro inesperado no logout: {CdUsuario}", cdUsuario);
            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao processar logout." });
        }
    }

    /// <summary>
    /// Obtém informações do usuário autenticado.
    /// </summary>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do usuário</returns>
    /// <response code="200">Dados do usuário retornados com sucesso</response>
    /// <response code="401">Não autorizado</response>
    /// <response code="404">Usuário não encontrado</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        // ✅ FIX: Usar claim customizado "cdusuario" em vez de ClaimTypes.NameIdentifier
        // ClaimTypes.NameIdentifier pode retornar o GUID do Sub, mas precisamos do CdUsuario
        var cdUsuario = User.FindFirstValue("cdusuario");

        if (string.IsNullOrWhiteSpace(cdUsuario))
        {
            return Unauthorized(new { error = "INVALID_TOKEN", message = "Token inválido." });
        }

        try
        {
            var query = new GetCurrentUserQuery(cdUsuario);
            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
            {
                return NotFound(new { error = result.Error.Code, message = result.Error.Message });
            }

            return Ok(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Erro ao obter dados do usuário: {CdUsuario}", cdUsuario);
            return StatusCode(500, new { error = "INTERNAL_ERROR", message = "Erro ao obter dados do usuário." });
        }
    }

    /// <summary>
    /// Valida se o token JWT atual ainda é válido (health check de autenticação).
    /// </summary>
    /// <returns>Status de validação</returns>
    /// <response code="200">Token válido</response>
    /// <response code="401">Token inválido ou expirado</response>
    [HttpGet("validate")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ValidateToken()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name);

        return Ok(new
        {
            valid = true,
            userId,
            userName,
            expiresAt = User.FindFirstValue("exp")
        });
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim();
        }

        if (Request.Headers.ContainsKey("X-Real-IP"))
        {
            return Request.Headers["X-Real-IP"].ToString();
        }

        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private string? GetUserAgent()
    {
        return Request.Headers.UserAgent.ToString();
    }
}