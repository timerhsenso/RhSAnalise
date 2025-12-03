using Microsoft.EntityFrameworkCore;
//using RhSensoERP.Modules.Avaliacoes.Core.Entities;
//using RhSensoERP.Modules.Avaliacoes.Core.Entities.Tabelas.Pessoal;
//using RhSensoERP.Modules.Esocial.Core.Entities;
using System.Reflection;

namespace RhSensoERP.Modules.Avaliacoes.Infrastructure.Persistence.Contexts
{
    public class AvaliacoesDbContext : DbContext
    {
        public AvaliacoesDbContext(DbContextOptions<AvaliacoesDbContext> options)
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
