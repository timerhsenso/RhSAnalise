// src/API/Controllers/DiagnosticsController.cs
#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace RhSensoERP.API.Controllers;


/// <summary>
/// Controller de diagnóstico e validações do ambiente (pré e pós-implantação).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "Diagnostics")] // ✅ ADICIONADO
public sealed class DiagnosticsController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(IConfiguration config, IWebHostEnvironment env, ILogger<DiagnosticsController> logger)
    {
        _config = config;
        _env = env;
        _logger = logger;
    }

    // ===============================
    // 1) BANCO DE DADOS / LATÊNCIA
    // ===============================

    /// <summary>Testa a conexão com o SQL Server, mede latência e retorna informações básicas.</summary>
    [HttpGet("database")]
    public async Task<IActionResult> TestDatabaseAsync()
    {
        var cn = _config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cn))
            return Ok(new { Status = "NotConfigured", Message = "ConnectionStrings:DefaultConnection ausente." });

        var sw = Stopwatch.StartNew();
        try
        {
            await using var conn = new SqlConnection(cn);
            await conn.OpenAsync();

            // consulta leve para obter versão
            await using var cmdVer = new SqlCommand("SELECT @@VERSION", conn);
            var version = (string?)await cmdVer.ExecuteScalarAsync() ?? "N/A";

            // contar tabelas (ignora erro de permissão)
            int tableCount = 0;
            try
            {
                await using var cmdTables = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'", conn);
                tableCount = Convert.ToInt32(await cmdTables.ExecuteScalarAsync());
            }
            catch { /* ignore */ }

            // medir round-trip com SELECT 1
            var pingSw = Stopwatch.StartNew();
            await using (var cmdPing = new SqlCommand("SELECT 1", conn))
            {
                await cmdPing.ExecuteScalarAsync();
            }
            pingSw.Stop();

            sw.Stop();
            return Ok(new
            {
                Status = "OK",
                DataSource = conn.DataSource,
                Database = conn.Database,
                Version = version,
                Tables = tableCount,
                OpenMs = sw.ElapsedMilliseconds,
                QueryMs = pingSw.ElapsedMilliseconds
            });
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "Falha ao conectar no banco.");
            return StatusCode(500, new { Status = "FAIL", ElapsedMs = sw.ElapsedMilliseconds, Error = ex.Message });
        }
    }

    /// <summary>Lista as últimas migrações do EF Core se a tabela __EFMigrationsHistory existir.</summary>
    [HttpGet("migrations")]
    public async Task<IActionResult> GetMigrationsAsync()
    {
        var cn = _config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cn))
            return Ok(new { Status = "NotConfigured" });

        try
        {
            await using var conn = new SqlConnection(cn);
            await conn.OpenAsync();

            // verifica existência da tabela
            const string existsSql = @"
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='__EFMigrationsHistory')
    SELECT 1
ELSE
    SELECT 0";
            await using var cmdExists = new SqlCommand(existsSql, conn);
            var exists = Convert.ToInt32(await cmdExists.ExecuteScalarAsync()) == 1;
            if (!exists)
                return Ok(new { Status = "NoHistoryTable" });

            const string topSql = @"SELECT TOP (20) MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId DESC";
            await using var cmd = new SqlCommand(topSql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<object>();
            while (await reader.ReadAsync())
            {
                list.Add(new
                {
                    MigrationId = reader.GetString(0),
                    ProductVersion = reader.GetString(1)
                });
            }

            return Ok(new { Status = "OK", Count = list.Count, Items = list });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao ler __EFMigrationsHistory.");
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    // ============
    // 2) REDIS
    // ============

    /// <summary>Testa o Redis usando a configuração em Redis:Configuration (StackExchange.Redis).</summary>
    [HttpGet("redis")]
    public async Task<IActionResult> TestRedisAsync()
    {
        var cfg = _config["Redis:Configuration"];
        if (string.IsNullOrWhiteSpace(cfg))
            return Ok(new { Status = "NotConfigured", Message = "Defina Redis:Configuration (ex: localhost:6379,abortConnect=false)" });

        try
        {
            var asm = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name?.Contains("StackExchange.Redis", StringComparison.OrdinalIgnoreCase) == true);

            if (asm is null)
                return Ok(new { Status = "NotAvailable", Message = "Pacote StackExchange.Redis não está referenciado." });

            var multiplexerType = asm.GetType("StackExchange.Redis.ConnectionMultiplexer");
            if (multiplexerType is null)
                return Ok(new { Status = "NotAvailable", Message = "Tipo ConnectionMultiplexer não encontrado." });

            dynamic mux = await (Task<dynamic>)multiplexerType
                .GetMethod("ConnectAsync", new[] { typeof(string) })!
                .Invoke(null, new object[] { cfg })!;

            var db = mux.GetDatabase();
            var key = $"diag:{Environment.MachineName}:{Guid.NewGuid():N}";
            var sw = Stopwatch.StartNew();
            await db.StringSetAsync(key, "ok", TimeSpan.FromSeconds(30));
            var val = (string?)await db.StringGetAsync(key);
            sw.Stop();

            mux.Dispose();

            return Ok(new { Status = val == "ok" ? "OK" : "FAIL", RoundtripMs = sw.ElapsedMilliseconds });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falha no teste do Redis.");
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    // ============
    // 3) SMTP
    // ============

    /// <summary>Envia um e-mail de teste usando a seção Diagnostics:Smtp.*</summary>
    [HttpPost("email")]
    public async Task<IActionResult> SendTestEmailAsync([FromQuery] string? to = null)
    {
        var host = _config["Diagnostics:Smtp:Host"];
        var port = _config.GetValue<int?>("Diagnostics:Smtp:Port");
        var user = _config["Diagnostics:Smtp:User"];
        var pass = _config["Diagnostics:Smtp:Pass"];
        var from = _config["Diagnostics:Smtp:From"];
        var toAddr = to ?? _config["Diagnostics:EmailTo"];

        if (string.IsNullOrWhiteSpace(host) || port is null || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(toAddr))
            return Ok(new { Status = "NotConfigured", Required = "Diagnostics:Smtp:{Host,Port,User,Pass,From} e/ou Diagnostics:EmailTo" });

        try
        {
            using var smtp = new SmtpClient(host, port.Value)
            {
                EnableSsl = _config.GetValue("Diagnostics:Smtp:EnableSsl", true),
                Credentials = string.IsNullOrWhiteSpace(user) ? null : new NetworkCredential(user, pass)
            };

            using var msg = new MailMessage(from, toAddr)
            {
                Subject = $"[Diag] Teste SMTP - {Environment.MachineName}",
                Body = $"Diagnóstico {DateTimeOffset.Now:yyyy-MM-dd HH:mm:ss zzz} - Ambiente: {_env.EnvironmentName}",
                IsBodyHtml = false
            };

            await smtp.SendMailAsync(msg);
            return Ok(new { Status = "OK", To = toAddr });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar e-mail de teste.");
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    // ======================
    // 4) LOGS (tail)
    // ======================

    /// <summary>Lê as últimas N linhas do arquivo de log (config em Diagnostics:LogFilePath).</summary>
    [HttpGet("logs")]
    public IActionResult TailLogs([FromQuery] int lines = 200)
    {
        var path = _config["Diagnostics:LogFilePath"];
        if (string.IsNullOrWhiteSpace(path))
            return Ok(new { Status = "NotConfigured", Message = "Defina Diagnostics:LogFilePath (ex: C:\\\\Logs\\\\app-.log)" });

        try
        {
            if (!System.IO.File.Exists(path))
                return Ok(new { Status = "NotFound", Path = path });

            var content = Tail(path, Math.Clamp(lines, 1, 2000));
            return Ok(new { Status = "OK", Path = path, Lines = content.Count, Content = content });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao ler o log.");
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    // ======================
    // 5) PERMISSÕES/PASTAS
    // ======================

    /// <summary>Testa permissão de escrita/leitura nas pastas (Logs, Uploads, Temp) definidas em Paths:*.</summary>
    [HttpPost("permissions")]
    public IActionResult TestPermissions()
    {
        var paths = new[]
        {
            ("Logs", _config["Paths:Logs"]),
            ("Uploads", _config["Paths:Uploads"]),
            ("Temp", _config["Paths:Temp"])
        };

        var results = new List<object>();
        foreach (var (name, path) in paths)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                results.Add(new { Name = name, Status = "NotConfigured" });
                continue;
            }

            try
            {
                Directory.CreateDirectory(path);
                var probe = Path.Combine(path, $"diag_{Guid.NewGuid():N}.txt");
                System.IO.File.WriteAllText(probe, "ok");
                var read = System.IO.File.ReadAllText(probe);
                System.IO.File.Delete(probe);

                results.Add(new { Name = name, Path = path, Status = read == "ok" ? "OK" : "FAIL" });
            }
            catch (Exception ex)
            {
                results.Add(new { Name = name, Path = path, Status = "FAIL", Error = ex.Message });
            }
        }

        return Ok(new { Results = results });
    }

    // ======================
    // 6) MEMÓRIA / GC
    // ======================

    /// <summary>Mostra uso de memória do processo e contadores de GC.</summary>
    [HttpGet("memory")]
    public IActionResult Memory()
    {
        var proc = Process.GetCurrentProcess();
        var ws = proc.WorkingSet64;
        var priv = proc.PrivateMemorySize64;

        var gcInfo = new
        {
            Gen0 = GC.CollectionCount(0),
            Gen1 = GC.CollectionCount(1),
            Gen2 = GC.CollectionCount(2),
            HeapBytes = GC.GetTotalMemory(false)
        };

        return Ok(new
        {
            Process = new
            {
                Pid = proc.Id,
                WorkingSetMB = Math.Round(ws / 1024d / 1024d, 2),
                PrivateMB = Math.Round(priv / 1024d / 1024d, 2),
                Threads = proc.Threads.Count
            },
            GC = gcInfo
        });
    }

    // ======================
    // 7) AMBIENTE / REDE
    // ======================

    /// <summary>Informações do ambiente: máquina, SO, .NET, IP, uptime.</summary>
    [HttpGet("environment")]
    public IActionResult GetEnvironmentInfo()
    {
        var asm = Assembly.GetExecutingAssembly();
        var version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ??
                      asm.GetName().Version?.ToString() ?? "N/A";

        return Ok(new
        {
            Machine = Environment.MachineName,
            OS = RuntimeInformation.OSDescription.Trim(),
            Arch = RuntimeInformation.ProcessArchitecture.ToString(),
            Framework = RuntimeInformation.FrameworkDescription,
            Environment = _env.EnvironmentName,
            Version = version,
            Uptime = GetUptime(),
            IpV4 = GetLocalIPv4()
        });
    }

    /// <summary>Ping ICMP para host (ex: 8.8.8.8).</summary>
    [HttpGet("ping")]
    public IActionResult TestPing([FromQuery] string host = "8.8.8.8")
    {
        try
        {
            using var ping = new Ping();
            var reply = ping.Send(host, 2000);
            return Ok(new { Host = host, Status = reply.Status.ToString(), RoundtripTime = reply.RoundtripTime });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    /// <summary>Resolve DNS de um domínio.</summary>
    [HttpGet("dns")]
    public async Task<IActionResult> ResolveDns([FromQuery] string host = "microsoft.com")
    {
        try
        {
            var entries = await Dns.GetHostAddressesAsync(host);
            var ips = entries.Select(e => new { Address = e.ToString(), Family = e.AddressFamily.ToString() });
            return Ok(new { Host = host, Addresses = ips });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Status = "FAIL", Error = ex.Message });
        }
    }

    // ======================
    // 8) OVERVIEW (resumo)
    // ======================

    /// <summary>Executa um resumo rápido do ambiente (útil para suporte/implantação).</summary>
    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        var envInfo = (GetEnvironmentInfo() as OkObjectResult)?.Value;

        // database
        object? dbInfo;
        {
            var r = await TestDatabaseAsync();
            dbInfo = (r as ObjectResult)?.Value;
        }

        // redis
        object? redisInfo;
        {
            var r = await TestRedisAsync();
            redisInfo = (r as ObjectResult)?.Value;
        }

        // permissões
        object? permInfo;
        {
            var r = TestPermissions();
            permInfo = (r as OkObjectResult)?.Value;
        }

        // migrações
        object? migInfo;
        {
            var r = await GetMigrationsAsync();
            migInfo = (r as ObjectResult)?.Value;
        }

        return Ok(new
        {
            Environment = envInfo,
            Database = dbInfo,
            Redis = redisInfo,
            Permissions = permInfo,
            Migrations = migInfo,
            Timestamp = DateTimeOffset.Now
        });
    }

    // ======================
    // Helpers
    // ======================

    private static string GetUptime()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    var asm = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name?.Equals("System.Diagnostics.PerformanceCounter", StringComparison.OrdinalIgnoreCase) == true);
                    var type = asm?.GetType("System.Diagnostics.PerformanceCounter");
                    if (type is not null)
                    {
                        using var pc = (IDisposable?)Activator.CreateInstance(type, "System", "System Up Time");
                        var nextValue = type.GetMethod("NextValue");
                        nextValue?.Invoke(pc, null);
                        var secondsObj = nextValue?.Invoke(pc, null);
                        if (secondsObj is float secondsF && secondsF > 0)
                            return TimeSpan.FromSeconds(secondsF).ToString(@"dd\.hh\:mm\:ss");
                    }
                }
                catch { /* fallback */ }
            }

            var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
            return uptime.ToString(@"dd\.hh\:mm\:ss");
        }
        catch
        {
            return "Unknown";
        }
    }

    private static string? GetLocalIPv4()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?.ToString();
        }
        catch { return null; }
    }

    private static List<string> Tail(string path, int lines)
    {
        var result = new List<string>(lines);
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs, Encoding.UTF8, true);
        var queue = new Queue<string>(lines);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine() ?? string.Empty;
            if (queue.Count == lines) queue.Dequeue();
            queue.Enqueue(line);
        }
        result.AddRange(queue);
        return result;
    }
}