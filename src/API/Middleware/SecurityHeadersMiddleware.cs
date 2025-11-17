// src/API/Middleware/SecurityHeadersMiddleware.cs

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate _next)
    {
        this._next = _next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ Content Security Policy
        context.Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: https:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "frame-ancestors 'none';");

        // ✅ X-Content-Type-Options
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // ✅ X-Frame-Options
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // ✅ X-XSS-Protection
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // ✅ Referrer-Policy
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // ✅ Permissions-Policy
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), microphone=(), camera=()");

        // ❌ Remover headers que expõem informações
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");
        context.Response.Headers.Remove("X-AspNet-Version");

        await _next(context);
    }
}

