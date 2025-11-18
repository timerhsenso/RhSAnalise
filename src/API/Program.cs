using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RhSensoERP.API.Middleware;
using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Infrastructure;
using RhSensoERP.Modules.GestaoDePessoas;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure;
using RhSensoERP.Shared.Infrastructure.Services;
using Serilog;
using System.Text;

// ============================================================================
// PROGRAM.CS - PONTO DE ENTRADA DA APLICAÇÃO RhSensoERP API
// ============================================================================
// Este arquivo configura toda a infraestrutura da aplicação:
// - Logging estruturado (Serilog)
// - Injeção de Dependência (DI) de todos os módulos
// - Autenticação JWT com validação de tokens
// - CORS para permitir requisições cross-origin
// - Swagger para documentação interativa da API
// - Middlewares de segurança e rate limiting
// - Pipeline de requisições HTTP
// ============================================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. CONFIGURAÇÃO DO SERILOG (LOGGING ESTRUTURADO)
// ============================================================================
// O Serilog substitui o logging padrão do .NET, oferecendo logs estruturados
// que facilitam análise, monitoramento e troubleshooting em produção.
//
// Características:
// - Enriquecimento automático com contexto (máquina, thread, timestamp)
// - Múltiplos destinos: Console (desenvolvimento) e Arquivo (produção)
// - Rotação diária de arquivos com retenção de 30 dias
// - Template customizado para melhor legibilidade
// ============================================================================
Log.Logger = new LoggerConfiguration()
    // Lê configurações adicionais do appsettings.json (níveis de log por namespace)
    .ReadFrom.Configuration(builder.Configuration)

    // Adiciona contexto automático aos logs (ex: CorrelationId, User)
    .Enrich.FromLogContext()

    // Adiciona nome da máquina (útil em ambientes com múltiplos servidores)
    .Enrich.WithMachineName()

    // Adiciona ID da thread (útil para debug de problemas de concorrência)
    .Enrich.WithThreadId()

    // SINK 1: Console - usado principalmente em desenvolvimento
    // Template compacto para facilitar leitura durante debug
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")

    // SINK 2: Arquivo - usado em produção para auditoria e troubleshooting
    // - Arquivos rotacionados diariamente (log-2025-01-15.txt)
    // - Mantém últimos 30 dias de logs
    // - Template detalhado com timestamp completo e timezone
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

// Substitui o logger padrão do ASP.NET Core pelo Serilog
builder.Host.UseSerilog();

// Logs iniciais para rastreamento de startup
Log.Information("🚀 Iniciando aplicação RhSensoERP API");
Log.Information("⚙️ Ambiente: {Environment}", builder.Environment.EnvironmentName);

// ============================================================================
// 2. CARREGAMENTO DE CONFIGURAÇÕES TIPADAS (OPTIONS PATTERN)
// ============================================================================
// O Options Pattern permite injetar configurações tipadas (appsettings.json)
// diretamente nas classes via IOptions<T> ou IOptionsSnapshot<T>.
//
// Vantagens:
// - Type-safety: erros de configuração são detectados em tempo de compilação
// - Intellisense: facilita descoberta de configurações disponíveis
// - Testabilidade: fácil mockar configurações em testes unitários
// - Validação: pode-se adicionar DataAnnotations para validar valores
// ============================================================================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<SecurityPolicySettings>(builder.Configuration.GetSection("SecurityPolicy"));

// ============================================================================
// 3. REGISTRO DE DEPENDÊNCIAS (DEPENDENCY INJECTION)
// ============================================================================
// Cada módulo da aplicação expõe um método de extensão (AddXxx) que registra
// todas as suas dependências (Repositories, Services, DbContext, etc).
//
// Arquitetura Modular:
// - Shared.Infrastructure: componentes compartilhados (Audit, UnitOfWork, etc)
// - Identity.Infrastructure: persistência de dados de autenticação
// - Identity.Application: serviços de negócio de autenticação
// - GestaoDePessoas: módulo de RH (colaboradores, cargos, departamentos)
//
// Cada módulo é isolado e pode evoluir independentemente.
// ============================================================================

// Infraestrutura compartilhada (Audit, Base Repository, UnitOfWork)
builder.Services.AddSharedInfrastructure();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

// Infraestrutura de Identity (ApplicationDbContext, Repositories)
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// Application layer de Identity (AuthService, TokenService, Validators)
builder.Services.AddIdentityApplication();

// Módulo de Gestão de Pessoas (RHU)
builder.Services.AddGestaoDePessoasModule(builder.Configuration);

// ============================================================================
// 4. CONFIGURAÇÃO DE CONTROLLERS E API EXPLORER
// ============================================================================
// AddControllers: Habilita suporte a MVC Controllers para APIs RESTful
// AddEndpointsApiExplorer: Expõe metadados dos endpoints para Swagger
// ============================================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ============================================================================
// 5. CONFIGURAÇÃO DE CORS (CROSS-ORIGIN RESOURCE SHARING)
// ============================================================================
// CORS é necessário quando o frontend (ex: Angular, React) está em um domínio
// diferente da API. Sem CORS, o browser bloqueia as requisições por segurança.
//
// ⚠️ IMPORTANTE: A ordem dos middlewares importa!
// UseCors() DEVE vir ANTES de UseAuthentication() e UseAuthorization()
//
// Configurações:
// - AllowedOrigins: lista de domínios permitidos (configurável por ambiente)
// - AllowAnyMethod: permite GET, POST, PUT, DELETE, etc
// - AllowAnyHeader: permite qualquer header HTTP
// - AllowCredentials: necessário para envio de cookies e tokens JWT
// ============================================================================

// Carrega origins do appsettings.json (pode variar por ambiente)
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

// ✅ FIX IMPORTANTE: Adiciona os próprios hosts da aplicação
// Isso permite que o Swagger (que roda no mesmo host) funcione corretamente
var allOrigins = new List<string>(corsOrigins)
{
    "https://localhost:7193",  // HTTPS local (desenvolvimento)
    "http://localhost:5174"    // HTTP local (desenvolvimento)
};

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        // Especifica exatamente quais origens são permitidas (mais seguro que AllowAnyOrigin)
        policy.WithOrigins(allOrigins.ToArray())

              // Permite todos os métodos HTTP (GET, POST, PUT, DELETE, PATCH, etc)
              .AllowAnyMethod()

              // Permite todos os headers (Authorization, Content-Type, etc)
              .AllowAnyHeader()

              // CRÍTICO: Permite envio de credenciais (JWT tokens, cookies)
              // Não pode ser usado junto com AllowAnyOrigin por segurança
              .AllowCredentials();
    });
});

// ============================================================================
// 6. CONFIGURAÇÃO DE AUTENTICAÇÃO JWT (JSON WEB TOKEN)
// ============================================================================
// JWT é o padrão de autenticação para APIs RESTful stateless.
// O token contém claims (dados do usuário) assinados com uma chave secreta.
//
// Fluxo:
// 1. Cliente faz login e recebe um access_token JWT
// 2. Cliente envia o token no header: Authorization: Bearer {token}
// 3. Middleware valida assinatura, expiração e claims
// 4. Se válido, popula HttpContext.User com os claims do token
//
// ⚠️ SEGURANÇA: A SecretKey NUNCA deve estar no código-fonte!
// - Desenvolvimento: usar User Secrets (dotnet user-secrets set)
// - Produção: usar variáveis de ambiente ou Azure KeyVault
// ============================================================================

// Carrega configurações de JWT do appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Validação crítica: SecretKey é obrigatória
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey não configurada. Configure via User Secrets (DEV) ou Environment Variables (PROD).");
}

// Converte a SecretKey para bytes (necessário para algoritmo HMAC-SHA256)
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        // Define JWT como esquema padrão para autenticação
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

        // Define JWT como esquema padrão para desafios (401 Unauthorized)
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        // Define JWT como esquema padrão geral
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        // Salva o token no AuthenticationProperties (útil para refresh tokens)
        options.SaveToken = true;

        // HTTPS é obrigatório em produção por segurança
        options.RequireHttpsMetadata = builder.Environment.IsProduction();

        // ====================================================================
        // PARÂMETROS DE VALIDAÇÃO DO TOKEN
        // ====================================================================
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // Valida se o Issuer (emissor) do token é confiável
            ValidateIssuer = true,

            // Valida se o Audience (público-alvo) do token está correto
            ValidateAudience = true,

            // Valida se o token não expirou (claim 'exp')
            ValidateLifetime = true,

            // Valida a assinatura do token com a chave secreta
            ValidateIssuerSigningKey = true,

            // Issuer esperado (deve corresponder ao gerado no TokenService)
            ValidIssuer = jwtSettings.Issuer,

            // Audience esperado (deve corresponder ao gerado no TokenService)
            ValidAudience = jwtSettings.Audience,

            // Chave secreta para validar a assinatura HMAC-SHA256
            IssuerSigningKey = new SymmetricSecurityKey(key),

            // Tolerância de clock skew (diferença de relógio entre servidores)
            // Evita rejeição de tokens por pequenas diferenças de horário
            ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes)
        };

        // ====================================================================
        // EVENTOS DO JWT BEARER
        // ====================================================================
        // Permitem customizar o comportamento em situações específicas
        options.Events = new JwtBearerEvents
        {
            // Disparado quando a autenticação falha
            OnAuthenticationFailed = context =>
            {
                // Detecta se o token expirou especificamente
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    // Adiciona header customizado para o cliente saber que deve fazer refresh
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },

            // Disparado quando a autenticação é desafiada (401)
            OnChallenge = context =>
            {
                // Previne a resposta padrão do middleware
                context.HandleResponse();

                // Define status code 401 Unauthorized
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                // Define content type como JSON
                context.Response.ContentType = "application/json";

                // Retorna uma resposta JSON padronizada e amigável
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "UNAUTHORIZED",
                    message = context.ErrorDescription ?? "Não autorizado. Token inválido ou expirado."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });

// Habilita o sistema de autorização (valida atributos [Authorize])
builder.Services.AddAuthorization();

// ============================================================================
// 7. CONFIGURAÇÃO DO SWAGGER (DOCUMENTAÇÃO INTERATIVA DA API)
// ============================================================================
// Swagger/OpenAPI gera documentação interativa da API automaticamente.
// Permite testar endpoints diretamente pelo browser.
//
// Recursos:
// - Listagem de todos os endpoints
// - Schemas de request/response
// - Teste de requisições com autenticação JWT
// - Agrupamento por controllers (tags)
//
// ⚠️ SEGURANÇA: Em produção, considere desabilitar ou proteger com autenticação
// ============================================================================
if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    builder.Services.AddSwaggerGen(options =>
    {
        // Metadados da API (aparecem na página inicial do Swagger)
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = builder.Configuration["Swagger:Title"] ?? "RhSensoERP API",
            Version = "v1",
            Description = builder.Configuration["Swagger:Description"] ?? "API do sistema de gestão RhSensoERP",
            Contact = new OpenApiContact
            {
                Name = builder.Configuration["Swagger:ContactName"] ?? "Equipe RhSenso",
                Email = builder.Configuration["Swagger:ContactEmail"] ?? "suporte@rhsenso.com.br"
            }
        });

        // ====================================================================
        // CONFIGURAÇÃO DE AUTENTICAÇÃO JWT NO SWAGGER
        // ====================================================================
        // Adiciona o botão "Authorize" no canto superior direito do Swagger
        // Permite informar o token JWT para testar endpoints protegidos
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT no formato: Bearer {seu token}"
        });

        // Aplica o esquema de segurança a todos os endpoints
        // Isso faz com que o Swagger envie o header Authorization automaticamente
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        // ====================================================================
        // ORGANIZAÇÃO POR TAGS (CONTROLLERS)
        // ====================================================================
        // Agrupa endpoints por controller no Swagger UI
        // Ex: AuthController → tag "Auth", UsuarioController → tag "Usuario"
        options.TagActionsBy(api =>
        {
            var groupName = api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default";
            return new[] { groupName };
        });

        // Inclui todos os endpoints na documentação
        options.DocInclusionPredicate((docName, apiDesc) => true);
    });
}

// ============================================================================
// 8. RATE LIMITING (PROTEÇÃO CONTRA ABUSE)
// ============================================================================
// Limita o número de requisições por IP/usuário para prevenir:
// - Ataques DDoS
// - Brute force em endpoints de login
// - Abuse de APIs públicas
//
// Configurações estão no middleware RateLimitingExtensions
// ============================================================================
builder.Services.AddRateLimiting();

// ============================================================================
// 9. BUILD DA APLICAÇÃO
// ============================================================================
// Constrói a aplicação com todas as configurações registradas acima
var app = builder.Build();

// ============================================================================
// 10. CONFIGURAÇÃO DO PIPELINE DE MIDDLEWARES
// ============================================================================
// ⚠️ ORDEM IMPORTA! Os middlewares são executados na ordem que são adicionados.
//
// Ordem recomendada pela Microsoft:
// 1. Exception Handling
// 2. HSTS
// 3. HTTPS Redirection
// 4. Static Files (se houver)
// 5. Routing
// 6. CORS ← ANTES de Authentication!
// 7. Authentication
// 8. Authorization
// 9. Custom Middlewares
// 10. Endpoints
// ============================================================================

// ====================================================================
// EXCEPTION HANDLING
// ====================================================================
// Desenvolvimento: mostra página detalhada de erro
// Produção: redireciona para endpoint genérico de erro
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");

    // HSTS: força HTTPS por 1 ano (header Strict-Transport-Security)
    app.UseHsts();
}

// ====================================================================
// SWAGGER UI (APENAS SE HABILITADO)
// ====================================================================
if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    // Expõe o JSON do OpenAPI em /swagger/v1/swagger.json
    app.UseSwagger();

    // Expõe a UI interativa em /swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RhSensoERP API v1");
        options.RoutePrefix = "swagger"; // Acesso: https://localhost:7193/swagger
        options.DocumentTitle = "RhSensoERP API Documentation";
    });
}

// ====================================================================
// HTTPS REDIRECTION
// ====================================================================
// Redireciona automaticamente HTTP → HTTPS (importante em produção)
app.UseHttpsRedirection();

// ====================================================================
// SERILOG REQUEST LOGGING
// ====================================================================
// Loga automaticamente todas as requisições HTTP (método, path, status, duração)
// Útil para auditoria e análise de performance
app.UseSerilogRequestLogging();

// ====================================================================
// CORS (CRITICAL: DEVE VIR ANTES DE AUTHENTICATION)
// ====================================================================
// Aplica a política de CORS configurada anteriormente
// Se vier depois de Authentication, o preflight OPTIONS não funcionará
app.UseCors("DefaultCorsPolicy");

// ====================================================================
// RATE LIMITING
// ====================================================================
// Aplica as regras de limitação de taxa configuradas
// Deve vir antes de Authentication para proteger o próprio endpoint de login
app.UseRateLimiter();


app.UseTenantResolution();

// ====================================================================
// AUTHENTICATION (VALIDA JWT TOKEN)
// ====================================================================
// Extrai e valida o token JWT do header Authorization
// Popula HttpContext.User com os claims do token
app.UseAuthentication();

// ====================================================================
// AUTHORIZATION (VALIDA [Authorize] ATTRIBUTES)
// ====================================================================
// Verifica se o usuário autenticado tem permissão para acessar o endpoint
// Deve sempre vir DEPOIS de Authentication
app.UseAuthorization();

// ====================================================================
// SECURITY HEADERS MIDDLEWARE (CUSTOM)
// ====================================================================
// Adiciona headers de segurança recomendados:
// - X-Content-Type-Options: nosniff
// - X-Frame-Options: DENY
// - X-XSS-Protection: 1; mode=block
// - Referrer-Policy: no-referrer
// - Content-Security-Policy
app.UseMiddleware<SecurityHeadersMiddleware>();

// ====================================================================
// ENDPOINTS (MAP CONTROLLERS)
// ====================================================================
// Mapeia os controllers registrados para os endpoints da API
app.MapControllers();

// ====================================================================
// HEALTH CHECK ENDPOINT
// ====================================================================
// Endpoint simples para verificar se a API está online
// Útil para load balancers, monitoramento e smoke tests
// Acesso: GET https://localhost:7193/health
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
})).AllowAnonymous(); // Não requer autenticação

// ============================================================================
// 11. INICIALIZAÇÃO E EXECUÇÃO DA APLICAÇÃO
// ============================================================================
// Try-catch garante que erros críticos no startup sejam logados
try
{
    Log.Information("✅ Aplicação RhSensoERP API iniciada com sucesso");

    // Log de configurações importantes para troubleshooting
    Log.Information("📊 SQL Logging: {Status}",
        builder.Configuration.GetValue<bool>("SqlLogging:Enabled") ? "HABILITADO" : "DESABILITADO");

    Log.Information("🌐 CORS: Permitindo origins: {Origins}", string.Join(", ", allOrigins));

    // Inicia o servidor Kestrel e aguarda requisições
    app.Run();
}
catch (Exception ex)
{
    // Log de erros fatais no startup (ex: falha ao conectar no banco)
    Log.Fatal(ex, "❌ Aplicação encerrada inesperadamente");
}
finally
{
    // Garante que todos os logs pendentes sejam escritos antes de encerrar
    Log.Information("🛑 Encerrando aplicação RhSensoERP API");
    Log.CloseAndFlush();
}