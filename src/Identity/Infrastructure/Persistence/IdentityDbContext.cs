using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

/// <summary>
/// DbContext do módulo de identidade/segurança.
/// </summary>
public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Funcao> Funcoes => Set<Funcao>();
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }

    // IUnitOfWork
    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        base.SaveChangesAsync(ct);
}
