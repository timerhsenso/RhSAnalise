// =============================================================================
// GERADOR FULL-STACK v3.0 - PROGRAM
// Aplicação ASP.NET Core para geração de código Full-Stack
// =============================================================================

using GeradorEntidades.Services;
using GeradorEntidades.TabSheet.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================================================================
// SERVICES
// =========================================================================

// Controllers com Views
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Usar camelCase para compatibilidade com JavaScript
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// Serviços de Geração
builder.Services.AddSingleton<DatabaseService>();
builder.Services.AddSingleton<CodeGeneratorService>();
builder.Services.AddSingleton<FullStackGeneratorService>();

// TabSheet Generator (NOVO)
builder.Services.AddScoped<TabSheetGeneratorService>();

// Logging
builder.Logging.AddConsole();

var app = builder.Build();

// =========================================================================
// PIPELINE
// =========================================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// =========================================================================
// STARTUP LOG
// =========================================================================

app.Logger.LogInformation("=================================================");
app.Logger.LogInformation("  GERADOR FULL-STACK v3.0 - RhSensoERP");
app.Logger.LogInformation("  Iniciado em: {Url}", app.Urls.FirstOrDefault() ?? "https://localhost:5001");
app.Logger.LogInformation("=================================================");

app.Run();