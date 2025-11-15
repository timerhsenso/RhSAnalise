using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence;

namespace RhSensoERP.API.Controllers;

/// <summary>
/// Controller para diagnósticos e testes do sistema.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous] // Remova em produção ou adicione [Authorize]
public class DiagnosticsController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<DiagnosticsController> _logger;
    private readonly IConfiguration _configuration;

    public DiagnosticsController(
        IdentityDbContext db,
        ILogger<DiagnosticsController> logger,
        IConfiguration configuration)
    {
        _db = db;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Testa a conexão com o banco de dados.
    /// </summary>
    [HttpGet("database")]
    public async Task<IActionResult> TestDatabaseAsync()
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            var dbName = _db.Database.GetDbConnection().Database;
            var connectionString = _db.Database.GetConnectionString();
            var providerName = _db.Database.ProviderName;

            var totalUsuarios = await _db.Usuarios.CountAsync();
            var totalSistemas = await _db.Sistemas.CountAsync();

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
    /// </summary>
    [HttpGet("test-sql-logging")]
    public async Task<IActionResult> TestSqlLogging()
    {
        _logger.LogInformation("🧪 Iniciando teste de SQL Logging...");

        try
        {
            // 1️⃣ Query rápida (< 5ms) - não deve aparecer se LogTrivialQueries = false
            _logger.LogInformation("1️⃣ Executando COUNT (query trivial)...");
            var count = await _db.Usuarios.CountAsync();

            // 2️⃣ Query com parâmetros
            _logger.LogInformation("2️⃣ Executando SELECT com parâmetros...");
            var usuario = await _db.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.CdUsuario == "ADMIN");

            // 3️⃣ Query complexa (JOIN)
            _logger.LogInformation("3️⃣ Executando query complexa com JOIN...");
            var userGroups = await _db.Set<UserGroup>()
                .AsNoTracking()
                .Include(ug => ug.GrupoDeUsuario)
                .Include(ug => ug.Sistema)
                .Where(ug => ug.CdUsuario == "ADMIN")
                .Take(5)
                .ToListAsync();

            // 4️⃣ Query com múltiplas condições
            _logger.LogInformation("4️⃣ Executando query com múltiplas condições...");
            var activeUsers = await _db.Usuarios
                .AsNoTracking()
                .Where(u => u.FlAtivo == 'S')
                .OrderBy(u => u.DcUsuario)
                .Take(10)
                .ToListAsync();

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
                .ToListAsync();

            _logger.LogInformation("✅ Teste de SQL Logging concluído com sucesso!");

            return Ok(new
            {
                message = "Teste de SQL Logging executado! Verifique os logs para ver as queries SQL.",
                results = new
                {
                    totalUsuarios = count,
                    usuarioEncontrado = usuario?.CdUsuario ?? "Não encontrado",
                    totalUserGroups = userGroups.Count,
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
    /// </summary>
    [HttpGet("test-slow-query")]
    public async Task<IActionResult> TestSlowQuery()
    {
        _logger.LogInformation("🐌 Executando query lenta propositalmente...");

        try
        {
            // Forçar query lenta com WAITFOR (apenas SQL Server)
            await _db.Database.ExecuteSqlRawAsync("WAITFOR DELAY '00:00:01'");

            _logger.LogInformation("✅ Query lenta executada. Deve ter gerado WARNING nos logs.");

            return Ok(new
            {
                message = "Query lenta executada! Verifique os logs para o alerta de performance.",
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
    /// </summary>
    [HttpPost("test-write-operations")]
    public async Task<IActionResult> TestWriteOperations()
    {
        _logger.LogInformation("✍️ Testando operações de escrita (INSERT/UPDATE/DELETE)...");

        try
        {
            // 1️⃣ INSERT
            _logger.LogInformation("1️⃣ Testando INSERT...");
            var testLog = new LoginAuditLog(
                Guid.NewGuid(),
                null,
                true,
                "127.0.0.1",
                "Test User Agent - SQL Logging"
            );
            _db.Set<LoginAuditLog>().Add(testLog);
            await _db.SaveChangesAsync();
            var insertedId = testLog.Id;

            // 2️⃣ UPDATE
            _logger.LogInformation("2️⃣ Testando UPDATE...");
            var logToUpdate = await _db.Set<LoginAuditLog>()
                .FirstOrDefaultAsync(l => l.Id == insertedId);

            if (logToUpdate != null)
            {
                // LoginAuditLog é imutável, então vamos buscar um usuário para atualizar
                var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.CdUsuario == "ADMIN");
                if (usuario != null)
                {
                    usuario.UpdatedAt = DateTime.UtcNow;
                    await _db.SaveChangesAsync();
                }
            }

            // 3️⃣ DELETE
            _logger.LogInformation("3️⃣ Testando DELETE...");
            var logToDelete = await _db.Set<LoginAuditLog>()
                .FirstOrDefaultAsync(l => l.Id == insertedId);

            if (logToDelete != null)
            {
                _db.Set<LoginAuditLog>().Remove(logToDelete);
                await _db.SaveChangesAsync();
            }

            _logger.LogInformation("✅ Operações de escrita testadas com sucesso!");

            return Ok(new
            {
                message = "Operações de escrita testadas! Verifique os logs.",
                operations = new
                {
                    insert = "✅ Executado",
                    update = "✅ Executado",
                    delete = "✅ Executado"
                },
                note = "Veja os emojis nos logs: ➕ (INSERT), ✏️ (UPDATE), 🗑️ (DELETE)"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro ao executar operações de escrita");
            return StatusCode(500, new
            {
                error = "Erro ao executar operações de escrita",
                message = ex.Message
            });
        }
    }

    /// <summary>
    /// Retorna a configuração atual do SqlLogging.
    /// </summary>
    [HttpGet("sql-logging-config")]
    public IActionResult GetSqlLoggingConfig()
    {
        var config = _configuration.GetSection("SqlLogging").Get<SqlLoggingOptions>();

        return Ok(new
        {
            sqlLogging = config,
            logLevel = _configuration["Logging:LogLevel:RhSensoERP.Shared.Infrastructure.Persistence.Interceptors.SqlLoggingInterceptor"]
        });
    }

    // ========================================
    // MÉTODOS AUXILIARES
    // ========================================

    private static string MaskConnectionString(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            return "N/A";

        // Mascarar password/senha
        return System.Text.RegularExpressions.Regex.Replace(
            connectionString,
            @"(Password|Pwd)=[^;]*",
            "$1=***MASKED***",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

/// <summary>
/// Classe para desserializar configuração (reutilizando do interceptor).
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