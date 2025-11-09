using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Shared.Core.Abstractions;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext, IUnitOfWork
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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }

    // =======================
    // IUnitOfWork Implementation
    // =======================
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        return await base.SaveChangesAsync(ct);
    }
}