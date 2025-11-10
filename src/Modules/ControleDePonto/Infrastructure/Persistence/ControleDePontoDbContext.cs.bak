// src/Modules/ControleDePonto/Infrastructure/Persistence/ControleDePontoDbContext.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RhSensoERP.Modules.ControleDePonto.Core.Configurations;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;
using RhSensoERP.Modules.ControleDePonto.Domain.Entities;
using RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;
using RhSensoERP.SharedKernel.Infrastructure.Persistence;
using RhSensoERP.SharedKernel.Infrastructure.Persistence.Interceptors;
using System.Reflection;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence
{
    public class ControleDePontoDbContext : BaseDbContext
    {
        private readonly AuditInterceptor _auditInterceptor;
        private readonly TenantInterceptor _tenantInterceptor;

        public ControleDePontoDbContext(
            DbContextOptions<ControleDePontoDbContext> options,
            AuditInterceptor auditInterceptor = null,
            TenantInterceptor tenantInterceptor = null) : base(options)
        {
            _auditInterceptor = auditInterceptor;
            _tenantInterceptor = tenantInterceptor;
        }

        // DbSets principais do módulo
        public DbSet<GloEventos> GloEventos { get; set; }
        public DbSet<RegistroPonto> RegistrosPonto { get; set; }
        public DbSet<JustificativaPonto> JustificativasPonto { get; set; }
        public DbSet<HorarioTrabalho> HorariosTrabalho { get; set; }
        public DbSet<EscalaTrabalho> EscalasTrabalho { get; set; }
        public DbSet<BancoHoras> BancosHoras { get; set; }
        public DbSet<FolhaPonto> FolhasPonto { get; set; }
        public DbSet<ImportacaoPonto> ImportacoesPonto { get; set; }
        public DbSet<ConfiguracaoPonto> ConfiguracoesPonto { get; set; }
        public DbSet<FeriadoPonto> FeriadosPonto { get; set; }
        public DbSet<AbonoPonto> AbonosPonto { get; set; }
        public DbSet<MarcacaoPonto> MarcacoesPonto { get; set; }
        public DbSet<AjustePonto> AjustesPonto { get; set; }
        public DbSet<AprovacaoPonto> AprovacoesPonto { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_auditInterceptor != null)
                optionsBuilder.AddInterceptors(_auditInterceptor);

            if (_tenantInterceptor != null)
                optionsBuilder.AddInterceptors(_tenantInterceptor);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configurações do módulo
            modelBuilder.ApplyConfiguration(new GloEventosConfiguration());
            modelBuilder.ApplyConfiguration(new RegistroPontoConfiguration());
            modelBuilder.ApplyConfiguration(new JustificativaPontoConfiguration());
            modelBuilder.ApplyConfiguration(new HorarioTrabalhoConfiguration());
            modelBuilder.ApplyConfiguration(new EscalaTrabalhoConfiguration());
            modelBuilder.ApplyConfiguration(new BancoHorasConfiguration());
            modelBuilder.ApplyConfiguration(new FolhaPontoConfiguration());
            modelBuilder.ApplyConfiguration(new ImportacaoPontoConfiguration());
            modelBuilder.ApplyConfiguration(new ConfiguracaoPontoConfiguration());
            modelBuilder.ApplyConfiguration(new FeriadoPontoConfiguration());
            modelBuilder.ApplyConfiguration(new AbonoPontoConfiguration());
            modelBuilder.ApplyConfiguration(new MarcacaoPontoConfiguration());
            modelBuilder.ApplyConfiguration(new AjustePontoConfiguration());
            modelBuilder.ApplyConfiguration(new AprovacaoPontoConfiguration());

            // Ou aplicar todas as configurações do assembly
            // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configurar schema padrão do módulo
            modelBuilder.HasDefaultSchema("ponto");

            // Aplicar convenções globais
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Configurar propriedades decimais
                var decimalProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

                foreach (var property in decimalProperties)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasPrecision(18, 2);
                }

                // Configurar propriedades DateTime para datetime2
                var dateTimeProperties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?));

                foreach (var property in dateTimeProperties)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasColumnType("datetime2");
                }
            }
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is not null &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                // Aqui você pode adicionar lógica adicional antes de salvar
                // Como validações, normalização de dados, etc.
            }
        }
    }
}