using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

/// <summary>
/// DbContext do módulo de identidade/segurança.
/// Contém os DbSets e aplica os mapeamentos Fluent via ApplyConfigurationsFromAssembly.
/// </summary>
public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    // =======================
    // DbSets (Tabelas)
    // =======================
    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Funcao> Funcoes => Set<Funcao>();
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();


    // =======================
    // Model Binding / Mappings
    // =======================
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as classes de configuração do assembly de Infrastructure
        // (ex.: SistemaConfiguration, FuncaoConfiguration, etc.)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
