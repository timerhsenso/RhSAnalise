// src/API/Controllers/SecurityMetricsEndpoint.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // ✅ necessário para CountAsync, etc.
using RhSensoERP.Identity.Infrastructure.Persistence;
using RhSensoERP.Identity.Domain.Entities; // ajuste se suas entidades estiverem em outro namespace

namespace RhSensoERP.API.Controllers
{
    [ApiController]
    [Route("api/security/metrics")]
    [Authorize(Roles = "Admin")]
    public class SecurityMetricsController : ControllerBase
    {
        private readonly IdentityDbContext _db;

        public SecurityMetricsController(IdentityDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetMetrics()
        {
            var last24h = DateTime.UtcNow.AddHours(-24);
            var last7d = DateTime.UtcNow.AddDays(-7);

            var metrics = new
            {
                // Tentativas de login
                loginAttempts = new
                {
                    last24h = await _db.Set<LoginAuditLog>()
                        .CountAsync(l => l.LoginAttemptAt >= last24h),

                    failedLast24h = await _db.Set<LoginAuditLog>()
                        .CountAsync(l => l.LoginAttemptAt >= last24h && !l.IsSuccess),

                    uniqueIPs = await _db.Set<LoginAuditLog>()
                        .Where(l => l.LoginAttemptAt >= last7d)
                        .Select(l => l.IpAddress)
                        .Distinct()
                        .CountAsync()
                },

                // Contas bloqueadas
                lockedAccounts = await _db.Set<UserSecurity>()
                    .CountAsync(u => u.LockoutEnd.HasValue && u.LockoutEnd > DateTime.UtcNow),

                // Senhas fracas (legado)
                weakPasswords = await _db.Usuarios
                    .CountAsync(u => u.SenhaUser != null),

                // 2FA habilitado
                twoFactorEnabled = await _db.Set<UserSecurity>()
                    .CountAsync(u => u.TwoFactorEnabled && !u.IsDeleted),

                // Refresh tokens ativos
                activeRefreshTokens = await _db.Set<RefreshToken>()
                    .CountAsync(t => !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
            };

            return Ok(metrics);
        }
    }
}
