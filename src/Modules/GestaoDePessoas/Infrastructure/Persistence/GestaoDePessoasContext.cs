// src/Modules/GestaoDePessoas/Infrastructure/Persistence/GestaoDePessoasContext.cs

using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;
using System.Reflection;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence
{
    public class GestaoDePessoasContext : DbContext
    {
        public GestaoDePessoasContext(DbContextOptions<GestaoDePessoasContext> options)
            : base(options)
        {
        }

        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Filial> Filiais { get; set; }
        public DbSet<Cargo> Cargos { get; set; }
        public DbSet<CentroCusto> CentrosCusto { get; set; }
        public DbSet<Municipio> Municipios { get; set; }
        public DbSet<Sindicato> Sindicatos { get; set; }
        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Agencia> Agencias { get; set; }
        public DbSet<MotivoRescisao> MotivosRescisao { get; set; }
        public DbSet<GrauInstrucao> GrausInstrucao { get; set; }
        public DbSet<Situacao> Situacoes { get; set; }
        public DbSet<VinculoEmpregaticio> VinculosEmpregaticio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as configurações do assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}