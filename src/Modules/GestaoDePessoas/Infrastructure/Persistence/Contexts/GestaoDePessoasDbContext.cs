using Microsoft.EntityFrameworkCore;
//using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;
//using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
using RhSensoERP.Modules.Esocial.Core.Entities;
using System.Reflection;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Contexts
{
    public class GestaoDePessoasDbContext : DbContext
    {
        public GestaoDePessoasDbContext(DbContextOptions<GestaoDePessoasDbContext> options)
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
