// src/API/Configuration/RateLimitingConfiguration.cs
using System.Threading.RateLimiting;

/// <summary>
/// Configuração de Rate Limiting para proteção contra abuse.
/// ✅ FASE 2: Adicionada política específica para diagnósticos
/// </summary>
public static class RateLimitingConfiguration
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // ====================================================================
            // 1. RATE LIMIT GLOBAL POR IP
            // ====================================================================
            // Aplica-se a TODAS as requisições que não têm política específica.
            // Protege contra DDoS e abuse geral da API.
            //
            // Configuração:
            // - 100 requisições por minuto por IP
            // - Janela fixa (reseta a cada minuto)
            // - Sem fila (rejeita imediatamente se exceder)
            // ====================================================================
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            // ====================================================================
            // 2. POLÍTICA "login" - Proteção contra Brute Force
            // ====================================================================
            // Aplicada ao endpoint POST /api/identity/auth/login
            //
            // Configuração:
            // - 5 tentativas a cada 5 minutos por IP
            // - Janela deslizante (mais preciso que janela fixa)
            // - 5 segmentos (granularidade de 1 minuto)
            // - Sem fila (rejeita imediatamente)
            //
            // Exemplo: Se usuário fizer 5 tentativas em 1 minuto, ficará bloqueado
            // por 4 minutos até que a janela deslize e libere slots.
            // ====================================================================
            options.AddPolicy("login", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetSlidingWindowLimiter(ipAddress, _ =>
                    new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(5),
                        SegmentsPerWindow = 5,
                        QueueLimit = 0
                    });
            });

            // ====================================================================
            // 3. POLÍTICA "refresh" - Renovação de Tokens
            // ====================================================================
            // Aplicada ao endpoint POST /api/identity/auth/refresh-token
            //
            // Configuração:
            // - 20 requisições por minuto por IP
            // - Mais permissiva que login (operação legítima e frequente)
            // - Janela fixa
            // - Sem fila
            //
            // Justificativa: Refresh tokens são renovados automaticamente por
            // aplicações frontend, então precisa ser mais permissivo.
            // ====================================================================
            options.AddPolicy("refresh", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
            });

            // ====================================================================
            // ✅ FASE 2: 4. POLÍTICA "diagnostics" - Endpoints de Diagnóstico
            // ====================================================================
            // Aplicada aos endpoints:
            // - GET /api/diagnostics/*
            // - GET /api/security/metrics
            //
            // Configuração:
            // - 10 requisições a cada 5 minutos por IP
            // - Janela fixa
            // - Sem fila
            //
            // Justificativa: Endpoints de diagnóstico são usados raramente e
            // apenas por administradores. Limite baixo previne abuse e
            // scanning automatizado.
            // ====================================================================
            options.AddPolicy("diagnostics", context =>
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 0
                    });
            });

            // ====================================================================
            // HANDLER DE REJEIÇÃO (429 TOO MANY REQUESTS)
            // ====================================================================
            // Executado quando uma requisição é rejeitada por rate limiting.
            // Retorna resposta JSON padronizada com informações úteis.
            // ====================================================================
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                // Tenta obter o tempo de retry dos metadados
                var retryAfter = context.Lease.GetAllMetadata()
                    .FirstOrDefault(m => m.Key == "RETRY_AFTER").Value;

                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    error = "RATE_LIMIT_EXCEEDED",
                    message = "Muitas tentativas. Aguarde alguns minutos antes de tentar novamente.",
                    retryAfter = retryAfter
                }, token);
            };
        });

        return services;
    }
}
