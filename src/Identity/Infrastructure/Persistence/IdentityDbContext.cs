using Microsoft.EntityFrameworkCore;
using RhSensoERP.Identity.Domain.Entities;
using RhSensoERP.Identity.Infrastructure.Persistence.Configurations;
using RhSensoERP.Shared.Core.Abstractions;
using RhSensoERP.Shared.Infrastructure.Persistence.Interceptors;

namespace RhSensoERP.Identity.Infrastructure.Persistence;

public sealed class IdentityDbContext : DbContext, IUnitOfWork
{
    private readonly AuditableEntityInterceptor _auditableEntityInterceptor;

    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor)
        : base(options)
    {
        _auditableEntityInterceptor = auditableEntityInterceptor;
    }

    public DbSet<Sistema> Sistemas => Set<Sistema>();
    public DbSet<Funcao> Funcoes => Set<Funcao>();
    public DbSet<BotaoFuncao> BotoesFuncao => Set<BotaoFuncao>();
    public DbSet<GrupoDeUsuario> GruposDeUsuario => Set<GrupoDeUsuario>();
    public DbSet<GrupoFuncao> GruposFuncoes => Set<GrupoFuncao>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    // ✅ FASE 5: Adicionar DbSet
    public DbSet<SecurityAuditLog> SecurityAuditLogs => Set<SecurityAuditLog>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Adicionar o interceptor de auditoria
        optionsBuilder.AddInterceptors(_auditableEntityInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);

        // ✅ FASE 5: Aplicar configuração (com prefixo SEG_)
        modelBuilder.ApplyConfiguration(new SecurityAuditLogConfiguration());

    }

    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken ct)
    {
        return await base.SaveChangesAsync(ct);
    }
}