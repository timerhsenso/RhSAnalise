using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.Esocial.Core.Entities;
using System.Reflection;


namespace RhSensoERP.Modules.Esocial.Infrastructure.Persistence.Contexts
{
    public class EsocialDbContext : DbContext
    {
        public EsocialDbContext(DbContextOptions<EsocialDbContext> options)
            : base(options)
        {
        }

        // ===== ENTIDADES =====
        public DbSet<Tab10Esocial> Tabs10Esocial { get; set; }
    


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as IEntityTypeConfiguration<> neste assembly (Configurations/*)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
