using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;
using System.IO;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

/// <summary>
/// Factory para criação do DbContext em tempo de design (CLI do EF Core).
/// Permite rodar 'dotnet ef migrations add' a partir do projeto Infrastructure.
/// </summary>
public sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        // Procura appsettings a partir da API (ajuste o caminho se necessário)
        var basePath = Directory.GetCurrentDirectory();

        // Tenta achar um appsettings subindo diretórios até chegar no root do repo
        // e prioriza o appsettings da API (onde costuma estar a ConnectionString).
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.json"), optional: true)
            .AddJsonFile(Path.Combine("..", "..", "API", "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=.;Database=bd_rhu_copenor;Trusted_Connection=True;TrustServerCertificate=true;";

        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        // Para design-time (migrations), criamos um interceptor fake
        // Ele não será usado durante a geração das migrations
        var fakeInterceptor = new AuditableEntityInterceptor(
            new FakeCurrentUser(),
            new FakeDateTimeProvider()
        );

        return new IdentityDbContext(optionsBuilder.Options, fakeInterceptor);
    }

    // Classes auxiliares para design-time apenas
    private sealed class FakeCurrentUser : RhSensoERP.Shared.Core.Abstractions.ICurrentUser
    {
        public string? UserId => "migration-user";
        public string? UserName => "Migration User";
    }

    private sealed class FakeDateTimeProvider : RhSensoERP.Shared.Core.Abstractions.IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}