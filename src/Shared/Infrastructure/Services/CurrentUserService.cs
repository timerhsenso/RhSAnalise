namespace RhSensoERP.Shared.Infrastructure.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using RhSensoERP.Shared.Core.Abstractions;

/// <summary>
/// Implementação do ICurrentUser usando HttpContext.
/// </summary>
public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?
        .User?
        .FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName => _httpContextAccessor.HttpContext?
        .User?
        .FindFirstValue(ClaimTypes.Name);
}