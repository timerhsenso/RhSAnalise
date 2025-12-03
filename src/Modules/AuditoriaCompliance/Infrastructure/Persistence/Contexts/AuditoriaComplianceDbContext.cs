using Microsoft.EntityFrameworkCore;
using System.Reflection;
//using RhSensoERP.Modules.AuditoriaCompliance.Core.Entities;

namespace RhSensoERP.Modules.AuditoriaCompliance.Infrastructure.Persistence.Contexts
{
    public class AuditoriaComplianceDbContext : DbContext
    {
        public AuditoriaComplianceDbContext(DbContextOptions<AuditoriaComplianceDbContext> options)
            : base(options)
        {
        }

        // ===== ENTIDADES JÁ EXISTENTES =====
        //public DbSet<Funcionario> Funcionarios { get; set; }
        //public DbSet<Empresa> Empresas { get; set; }
        //public DbSet<Filial> Filiais { get; set; }
        //public DbSet<Cargo> Cargos { get; set; }
        //public DbSet<CentroCusto> CentrosCusto { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as IEntityTypeConfiguration<> neste assembly (Configurations/*)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
