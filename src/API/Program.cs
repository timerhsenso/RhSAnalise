using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RhSensoERP.Identity.Application;
using RhSensoERP.Identity.Application.Configuration;
using RhSensoERP.Identity.Infrastructure;
using RhSensoERP.Shared.Infrastructure;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==================== SERILOG ====================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

Log.Information("🚀 Iniciando aplicação RhSensoERP API");
Log.Information("⚙️ Ambiente: {Environment}", builder.Environment.EnvironmentName);

// ==================== CONFIGURATION OPTIONS ====================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<SecurityPolicySettings>(builder.Configuration.GetSection("SecurityPolicy"));

// ==================== DEPENDENCY INJECTION ====================
builder.Services.AddSharedInfrastructure();
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.AddIdentityApplication();

// ==================== CONTROLLERS ====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ==================== CORS ====================
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        if (corsOrigins.Length > 0)
        {
            policy.WithOrigins(corsOrigins);
        }
        else
        {
            policy.AllowAnyOrigin();
        }

        policy.AllowAnyMethod()
              .AllowAnyHeader();

        if (corsOrigins.Length > 0)
        {
            policy.AllowCredentials();
        }
    });
});

// ==================== JWT AUTHENTICATION ====================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

if (jwtSettings == null || string.IsNullOrWhiteSpace(jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey não configurada. Configure via User Secrets (DEV) ou Environment Variables (PROD).");
}

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.FromMinutes(jwtSettings.ClockSkewMinutes)
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    error = "UNAUTHORIZED",
                    message = context.ErrorDescription ?? "Não autorizado. Token inválido ou expirado."
                });

                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization();

// ==================== SWAGGER ====================
if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    builder.Services.AddSwaggerGen(options =>
    {
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

        // Configuração JWT no Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira o token JWT no formato: Bearer {seu token}"
        });

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

        // Tags para organização
        options.TagActionsBy(api =>
        {
            var groupName = api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] ?? "Default";
            return new[] { groupName };
        });

        options.DocInclusionPredicate((docName, apiDesc) => true);
    });
}

// ==================== BUILD APP ====================
var app = builder.Build();

// ==================== MIDDLEWARE PIPELINE ====================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

if (builder.Configuration.GetValue<bool>("Features:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "RhSensoERP API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "RhSensoERP API Documentation";
    });
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();

app.UseCors("DefaultCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check básico
app.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    environment = app.Environment.EnvironmentName
})).AllowAnonymous();

// ==================== RUN ====================
try
{
    Log.Information("✅ Aplicação RhSensoERP API iniciada com sucesso");
    Log.Information("📊 SQL Logging: {Status}",
        builder.Configuration.GetValue<bool>("SqlLogging:Enabled") ? "HABILITADO" : "DESABILITADO");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Aplicação encerrada inesperadamente");
}
finally
{
    Log.Information("🛑 Encerrando aplicação RhSensoERP API");
    Log.CloseAndFlush();
}