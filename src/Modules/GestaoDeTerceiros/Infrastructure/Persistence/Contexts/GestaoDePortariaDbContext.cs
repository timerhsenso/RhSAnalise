using Microsoft.EntityFrameworkCore;
using System.Reflection;
//using RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Infrastructure.Persistence.Contexts
{
    public class GestaoDeTerceirosDbContext : DbContext
    {
        public GestaoDeTerceirosDbContext(DbContextOptions<GestaoDeTerceirosDbContext> options)
            : base(options)
        {
        }

        // ===== ENTIDADES JÁ EXISTENTES =====
        //public DbSet<Funcionario> Funcionarios { get; set; }
        //public DbSet<Empresa> Empresas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as IEntityTypeConfiguration<> neste assembly (Configurations/*)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
