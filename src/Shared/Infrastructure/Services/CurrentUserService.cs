// src/Shared/Infrastructure/Services/CurrentUserService.cs
using Microsoft.AspNetCore.Http;
using RhSensoERP.Shared.Core.Abstractions;
using System.Security.Claims;

namespace RhSensoERP.Shared.Infrastructure.Services;

public sealed class CurrentUserService : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public string? UserName =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
}