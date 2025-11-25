// ============================================================================
// RHSENSOERP API - PROGRAM.CS
// ============================================================================
// Arquivo: src/API/Program.cs
// Projeto: RhSensoERP - Sistema de Gestão de Recursos Humanos
// Versão: 1.0.0
// Última atualização: Novembro 2025
//
// DESCRIÇÃO:
// Ponto de entrada da aplicação ASP.NET Core Web API.
// Configura toda a infraestrutura, middlewares, serviços e pipeline HTTP.
//
// PRINCIPAIS CONFIGURAÇÕES:
// 1. Logging estruturado (Serilog)
// 2. Injeção de Dependência (DI) de todos os módulos
// 3. Autenticação JWT com validações de segurança
// 4. CORS para permitir requisições cross-origin
// 5. Swagger/OpenAPI para documentação interativa
// 6. Rate Limiting configurável por ambiente
// 7. Middlewares customizados de segurança
// 8. Background Services para tarefas agendadas
//
// ✅ CORREÇÕES DE SEGURANÇA APLICADAS:
// - Validações rigorosas de JWT (SecretKey, comprimento, termos proibidos)
// - Rate Limiting configurável via appsettings.json (linha 122)
// - HTTPS obrigatório em produção
// - Security Headers (X-Content-Type-Options, X-Frame-Options, etc)
// - Auditoria de segurança com limpeza automática
//
// ARQUITETURA:
// - Modular: cada módulo (Identity, GestaoDePessoas) é isolado
// - Clean Architecture: separação clara entre camadas
// - Options Pattern: configurações tipadas e testáveis
// - Dependency Injection: baixo acoplamento e alta testabilidade
// ============================================================================

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RhSensoERP.API.BackgroundServices;
using RhSensoERP.API.Configuration;
using RhSensoERP.API.Middleware;
using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Application.Services;
using RhSensoERP.Identity.Infrastructure;

// ❌ COMENTADO TEMPORARIAMENTE - Erro no EF mapeamento
/////using RhSensoERP.Modules.GestaoDePessoas;
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
// ✅ FASE 2: CONFIGURAÇÃO DE RATE LIMITING (Options Pattern)
// ============================================================================
// Registra as configurações de Rate Limiting do appsettings.json no DI container.
// Permite que RateLimitingConfiguration.cs leia as configurações via IOptions<T>.
//
// Estrutura esperada no appsettings.json:
// {
//   "RateLimit": {
//     "Global": {
//       "PermitLimit": 100,
//       "WindowMinutes": 1,
//       "WindowType": "Fixed"
//     },
//     "Policies": {
//       "login": { ... },
//       "refresh": { ... },
//       "diagnostics": { ... }
//     }
//   }
// }
//
// BENEFÍCIO: Permite configuração diferente por ambiente (dev/staging/prod)
// sem necessidade de recompilação.
// ============================================================================
builder.Services.Configure<RateLimitSettings>(builder.Configuration.GetSection("RateLimit"));

// ============================================================================
// ✅ VALIDAÇÃO: Verificar se RateLimitSettings foi carregado
// ============================================================================
// Garante que a seção "RateLimit" existe no appsettings.json.
// Se não existir, a aplicação usará valores default do RateLimitingConfiguration.
// ============================================================================
var rateLimitConfig = builder.Configuration.GetSection("RateLimit");
if (!rateLimitConfig.Exists())
{
    Log.Warning("⚠️ Seção 'RateLimit' não encontrada no appsettings.json. Usando valores default.");
}
else
{
    Log.Information("✅ Configuração de Rate Limiting carregada do appsettings.json");
}

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

// ❌ COMENTADO TEMPORARIAMENTE - Erro no EF mapeamento
//builder.Services.AddGestaoDePessoasModule(builder.Configuration);

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

// ============================================================================
// ✅ FASE 1 - VALIDAÇÃO CRÍTICA DE SEGURANÇA
// ============================================================================
// Validação obrigatória: SecretKey DEVE estar configurada
if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "CRITICAL SECURITY ERROR: JwtSettings:SecretKey não configurada!\n\n" +
        "Para configurar:\n" +
        "  - Desenvolvimento: dotnet user-secrets set \"JwtSettings:SecretKey\" \"SUA_CHAVE_AQUI\"\n" +
        "  - Produção: Defina variável de ambiente JwtSettings__SecretKey\n\n" +
        "Gerar chave segura: openssl rand -base64 64");
}

// ✅ FASE 1 - VALIDAÇÕES ESPECÍFICAS DE PRODUÇÃO
if (builder.Environment.IsProduction())
{
    // Validação 1: Chave deve ser forte (mínimo 64 caracteres)
    if (jwtSettings.SecretKey.Length < 64)
    {
        throw new InvalidOperationException(
            "CRITICAL: Em produção, JwtSettings:SecretKey deve ter no mínimo 64 caracteres!\n" +
            "Chave atual tem apenas " + jwtSettings.SecretKey.Length + " caracteres.");
    }

    // Validação 2: Prevenir uso de chaves genéricas
    var forbiddenTerms = new[] { "Development", "Example", "Test", "Demo", "Sample", "Desenvolvimento" };
    if (forbiddenTerms.Any(term => jwtSettings.SecretKey.Contains(term, StringComparison.OrdinalIgnoreCase)))
    {
        throw new InvalidOperationException(
            "CRITICAL: JwtSettings:SecretKey em produção não pode conter termos genéricos!\n" +
            "Termos proibidos: " + string.Join(", ", forbiddenTerms));
    }

    // Validação 3: Connection string não pode usar credenciais default
    var connString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (connString?.Contains("sa", StringComparison.OrdinalIgnoreCase) == true ||
        connString?.Contains("Password=123") == true ||
        connString?.Contains("Password=admin") == true)
    {
        throw new InvalidOperationException(
            "CRITICAL: Connection string em produção não pode usar credenciais default (sa, 123, admin)!");
    }

    Log.Information("✅ Validações de segurança de produção concluídas com sucesso");
}

Log.Information("✅ Validação de JwtSettings concluída com sucesso");

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

        // ✅ FASE 1 - HTTPS obrigatório em produção
        options.RequireHttpsMetadata = builder.Environment.IsProduction();

        // ✅ FASE 1 - VALIDAÇÃO EXTRA: Garantir que HTTPS está ativo em produção
        if (builder.Environment.IsProduction() && !options.RequireHttpsMetadata)
        {
            throw new InvalidOperationException(
                "CRITICAL: RequireHttpsMetadata DEVE ser true em produção!");
        }

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
// SERVIÇOS DE SEGURANÇA E AUDITORIA
// ============================================================================
// Registro de serviços relacionados a segurança e auditoria de operações.
// ============================================================================

// 1. Serviço de auditoria de segurança
// Responsável por registrar eventos de segurança (login, falhas, etc)
builder.Services.AddScoped<ISecurityAuditService, SecurityAuditService>();

// 2. Background Service para limpeza automática de logs de auditoria
// Executa periodicamente para remover logs antigos conforme configuração
// em "AuditCleanup:RetentionDays" do appsettings.json
builder.Services.AddHostedService<AuditCleanupBackgroundService>();

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
// - Ataques DDoS (Distributed Denial of Service)
// - Brute force em endpoints de login
// - Abuse de APIs públicas
// - Scraping automatizado
//
// ✅ CONFIGURAÇÃO FLEXÍVEL:
// As regras de rate limiting são carregadas do appsettings.json via
// RateLimitSettings (registrado na linha 122). Isso permite:
// - Ajustar limites sem recompilar
// - Configuração diferente por ambiente (dev/staging/prod)
// - Resposta rápida a ataques
//
// POLÍTICAS CONFIGURADAS:
// - Global: limite geral para todos os endpoints
// - login: proteção contra brute force (5-20 tentativas/5min)
// - refresh: renovação de tokens (20-30 req/min)
// - diagnostics: endpoints administrativos (10-20 req/5min)
//
// Implementação: RateLimitingConfiguration.cs
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

    // ✅ FASE 1 - HSTS: força HTTPS por 1 ano (header Strict-Transport-Security)
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
// ✅ FASE 1 - HTTPS REDIRECTION (FORÇADO EM PRODUÇÃO)
// ====================================================================
// Redireciona automaticamente HTTP → HTTPS (importante em produção)
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
    Log.Information("✅ HTTPS Redirection habilitado (produção)");
}
else
{
    // Em desenvolvimento, também redireciona mas sem log crítico
    app.UseHttpsRedirection();
}

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
// Aplica as regras de limitação de taxa configuradas via RateLimitSettings.
// 
// ⚠️ ORDEM IMPORTANTE: Deve vir ANTES de Authentication para proteger
// o próprio endpoint de login contra brute force.
//
// Comportamento:
// - Requisições dentro do limite: passam normalmente
// - Requisições acima do limite: retornam 429 (Too Many Requests)
// - Resposta inclui JSON com erro e tempo de retry
// ====================================================================
app.UseRateLimiter();

// ====================================================================
// TENANT RESOLUTION (MULTI-TENANCY)
// ====================================================================
// Middleware customizado para resolução de tenant (empresa/organização).
// 
// Funcionalidade:
// - Identifica qual tenant está fazendo a requisição
// - Pode usar header, subdomain, ou claim do JWT
// - Popula ITenantContext para uso nos repositories
//
// Benefícios:
// - Isola dados entre diferentes empresas/organizações
// - Permite SaaS multi-tenant
// - Segurança: previne acesso cruzado entre tenants
// ====================================================================
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

    // ========================================================================
    // LOGS DE CONFIGURAÇÕES IMPORTANTES (TROUBLESHOOTING)
    // ========================================================================
    // Exibe status de configurações críticas para facilitar diagnóstico.
    // ========================================================================

    // SQL Logging
    Log.Information("📊 SQL Logging: {Status}",
        builder.Configuration.GetValue<bool>("SqlLogging:Enabled") ? "HABILITADO" : "DESABILITADO");

    // Rate Limiting
    var rateLimitEnabled = rateLimitConfig.Exists();
    Log.Information("⏱️ Rate Limiting: {Status}",
        rateLimitEnabled ? "CONFIGURADO (appsettings.json)" : "DEFAULT (hardcoded)");
    
    if (rateLimitEnabled)
    {
        var globalLimit = builder.Configuration.GetValue<int>("RateLimit:Global:PermitLimit");
        var loginLimit = builder.Configuration.GetValue<int>("RateLimit:Policies:login:PermitLimit");
        Log.Information("🛡️ Limites: Global={GlobalLimit} req/min, Login={LoginLimit} tentativas",
            globalLimit, loginLimit);
    }

    // CORS
    Log.Information("🌐 CORS: Permitindo origins: {Origins}", string.Join(", ", allOrigins));

    // HTTPS
    Log.Information("🔒 HTTPS: {Status}",
        app.Environment.IsProduction() ? "OBRIGATÓRIO (produção)" : "Opcional (desenvolvimento)");

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
