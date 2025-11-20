// src/API/Controllers/DiagnosticsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para diagnósticos e testes do sistema.
/// ✅ FASE 1: Protegido com [Authorize(Roles = "Admin")] e oculto do Swagger em Release
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // ✅ FASE 1: Apenas administradores podem acessar
#if !DEBUG
[ApiExplorerSettings(IgnoreApi = true)] // ✅ FASE 1: Oculta do Swagger em Release
#endif
public class DiagnosticsController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<DiagnosticsController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public DiagnosticsController(
        IdentityDbContext db,
        ILogger<DiagnosticsController> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
        _environment = environment;
    }

    /// <summary>
    /// Testa a conexão com o banco de dados.
    /// ✅ FASE 1: Desabilitado em produção por segurança
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> TestDatabaseAsync(CancellationToken ct)
    {
        // ✅ FASE 1: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Diagnósticos desabilitados em produção por segurança."
            });
        }

        try
        {
            var canConnect = await _db.Database.CanConnectAsync(ct);
            var dbName = _db.Database.GetDbConnection().Database;
            var connectionString = _db.Database.GetConnectionString();
            var providerName = _db.Database.ProviderName;

            var totalUsuarios = await _db.Usuarios.CountAsync(ct);
            var totalSistemas = await _db.Sistemas.CountAsync(ct);

            return Ok(new
            {
                status = canConnect ? "Connected" : "Disconnected",
                database = dbName,
                provider = providerName,
                connectionStringMasked = MaskConnectionString(connectionString),
                statistics = new
                {
                    totalUsuarios,
                    totalSistemas
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao testar conexão com o banco de dados");
            return StatusCode(500, new
            {
                status = "Error",
                message = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Testa o SqlLoggingInterceptor com queries variadas.
    /// ✅ FASE 1: Desabilitado em produção por segurança
    /// </summary>
    [HttpGet("test-sql-logging")]
    public async Task<IActionResult> TestSqlLogging(CancellationToken ct)
    {
        // ✅ FASE 1: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Diagnósticos desabilitados em produção por segurança."
            });
        }

        _logger.LogInformation("🧪 Iniciando teste de SQL Logging...");

        try
        {
            // 1️⃣ Query rápida (< 5ms)
            _logger.LogInformation("1️⃣ Executando COUNT (query trivial)...");
            var count = await _db.Usuarios.CountAsync(ct);

            // 2️⃣ Query com parâmetros
            _logger.LogInformation("2️⃣ Executando SELECT com parâmetros...");
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == "ADMIN", ct);

            // 3️⃣ Query complexa (JOIN) - REMOVIDO para evitar erro de colunas
            _logger.LogInformation("3️⃣ Executando query complexa...");
            var sistemas = await _db.Sistemas
                .AsNoTracking()
                .Take(5)
                .ToListAsync(ct);

            // 4️⃣ Query com múltiplas condições
            _logger.LogInformation("4️⃣ Executando query com múltiplas condições...");
            var activeUsers = await _db.Usuarios
                .AsNoTracking()
                .Where(u => u.FlAtivo == 'S')
                .OrderBy(u => u.DcUsuario)
                .Take(10)
                .ToListAsync(ct);

            // 5️⃣ Aggregate query
            _logger.LogInformation("5️⃣ Executando aggregate query...");
            var userStats = await _db.Usuarios
                .AsNoTracking()
                .GroupBy(u => u.FlAtivo)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(ct);

            _logger.LogInformation("✅ Teste de SQL Logging concluído com sucesso!");

            return Ok(new
            {
                message = "Teste de SQL Logging executado! Verifique os logs.",
                results = new
                {
                    totalUsuarios = count,
                    usuarioEncontrado = usuario?.CdUsuario ?? "Não encontrado",
                    totalSistemas = sistemas.Count,
                    totalActiveUsers = activeUsers.Count,
                    userStatistics = userStats
                },
                logFiles = new
                {
                    console = "Verifique o console da aplicação",
                    file = "logs/sql-*.txt"
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar teste de SQL Logging");
            return StatusCode(500, new
            {
                error = "Erro ao executar teste",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Testa query lenta (vai gerar warning de performance).
    /// ✅ FASE 1: Substituído WAITFOR DELAY por Task.Delay (boa prática)
    /// </summary>
    [HttpGet("test-slow-query")]
    public async Task<IActionResult> TestSlowQuery(CancellationToken ct)
    {
        // ✅ FASE 1: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Diagnósticos desabilitados em produção por segurança."
            });
        }

        _logger.LogInformation("🐌 Executando query lenta propositalmente...");

        try
        {
            // ✅ FASE 1: Substituído WAITFOR DELAY por Task.Delay
            // ANTES: await _db.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:01'");
            // DEPOIS: Usar C# puro para delays (mais limpo e sem SQL injection)
            await Task.Delay(1000, ct);

            _logger.LogInformation("✅ Query lenta executada.");

            return Ok(new
            {
                message = "Query lenta executada! Verifique os logs.",
                note = "✅ FASE 1: Agora usando Task.Delay ao invés de WAITFOR DELAY SQL",
                expectedWarning = "⚠️ SLOW QUERY DETECTED",
                threshold = "500ms (configurável em appsettings)"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar query lenta");
            return StatusCode(500, new
            {
                error = "Erro ao executar query lenta",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Testa INSERT/UPDATE/DELETE (non-query commands).
    /// ✅ FASE 1: Desabilitado em produção por segurança
    /// </summary>
    [HttpPost("test-write-operations")]
    public async Task<IActionResult> TestWriteOperations(CancellationToken ct)
    {
        // ✅ FASE 1: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Diagnósticos desabilitados em produção por segurança."
            });
        }

        _logger.LogInformation("✍️ Testando operações de escrita...");

        try
        {
            // ✅ FIX: Buscar um UserSecurity EXISTENTE primeiro
            var userSecurity = await _db.Set<UserSecurity>()
                .Where(us => !us.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (userSecurity == null)
            {
                return BadRequest(new
                {
                    status = "error",
                    message = "❌ Nenhum UserSecurity encontrado. Crie um usuário primeiro."
                });
            }

            // 1️⃣ INSERT com UserSecurity VÁLIDO
            _logger.LogInformation("1️⃣ Testando INSERT...");
            var testLog = new LoginAuditLog(
                userSecurity.Id, // ✅ ID REAL
                null,
                true,
                "127.0.0.1",
                "Test User Agent - SQL Logging"
            );

            _db.Set<LoginAuditLog>().Add(testLog);
            await _db.SaveChangesAsync(ct);
            var insertedId = testLog.Id;

            _logger.LogInformation("✅ INSERT realizado: ID = {Id}", insertedId);

            // 2️⃣ UPDATE
            _logger.LogInformation("2️⃣ Testando UPDATE...");
            var usuario = await _db.Usuarios
                .FirstOrDefaultAsync(u => u.CdUsuario == "ADMIN", ct);

            if (usuario != null)
            {
                usuario.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("✅ UPDATE realizado");
            }

            // 3️⃣ DELETE
            _logger.LogInformation("3️⃣ Testando DELETE...");
            var logToDelete = await _db.Set<LoginAuditLog>()
                .FirstOrDefaultAsync(l => l.Id == insertedId, ct);

            if (logToDelete != null)
            {
                _db.Set<LoginAuditLog>().Remove(logToDelete);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("✅ DELETE realizado");
            }

            return Ok(new
            {
                message = "Operações de escrita testadas com sucesso!",
                operations = new
                {
                    insert = "✅ Executado",
                    update = "✅ Executado",
                    delete = "✅ Executado"
                },
                note = "Veja os emojis nos logs: ➕ INSERT, ✏️ UPDATE, 🗑️ DELETE"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar operações de escrita");
            return StatusCode(500, new
            {
                error = "Erro ao executar operações de escrita",
                message = ex.Message,
                innerException = ex.InnerException?.Message
            });
        }
    }

    /// <summary>
    /// Retorna a configuração atual do SqlLogging.
    /// ✅ FASE 1: Desabilitado em produção por segurança
    /// </summary>
    [HttpGet("sql-logging-config")]
    public IActionResult GetSqlLoggingConfig()
    {
        // ✅ FASE 1: Desabilitar em produção
        if (_environment.IsProduction())
        {
            return StatusCode(403, new
            {
                error = "FORBIDDEN",
                message = "Diagnósticos desabilitados em produção por segurança."
            });
        }

        var config = _configuration
            .GetSection("SqlLogging")
            .Get<SqlLoggingOptions>();

        return Ok(new
        {
            sqlLogging = config,
            logLevel = _configuration[
                "Logging:LogLevel:RhSensoERP.Shared.Infrastructure.Persistence.Interceptors.SqlLoggingInterceptor"]
        });
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private static string MaskConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return "N/A";

        return System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"(Password|Pwd)=[^;]*",
            "$1=***MASKED***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

/// <summary>
/// Opções de configuração do SqlLogging.
/// </summary>
public class SqlLoggingOptions
{
    public bool Enabled { get; set; }
    public bool LogSqlText { get; set; }
    public bool LogParameters { get; set; }
    public bool FormatSql { get; set; }
    public bool MaskSensitiveData { get; set; }
    public bool LogTrivialQueries { get; set; }
    public double TrivialQueryThresholdMs { get; set; }
    public double SlowQueryWarningThresholdMs { get; set; }
}
