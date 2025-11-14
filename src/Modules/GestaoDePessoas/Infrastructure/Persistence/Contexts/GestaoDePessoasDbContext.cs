using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;
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

        // ===== ENTIDADES DO LOTE ATUAL =====
        public DbSet<LotacaoTributaria> LotacoesTributarias { get; set; }
        public DbSet<MotivoOcorrenciaFrequencia> MotivosOcorrenciaFrequencia { get; set; }
        public DbSet<Tab21Esocial> Tabs21Esocial { get; set; }
        public DbSet<Tab8Esocial> Tabs8Esocial { get; set; }
        public DbSet<Tcbo> Cbos { get; set; }
        public DbSet<TabelaSalarial> TabelasSalariais { get; set; }

        // ===== NOVOS CADASTROS DE REFERÊNCIA (agora com FK em LotacaoTributaria) =====
        public DbSet<Tab10Esocial> Tabs10Esocial { get; set; }
        public DbSet<Tab4Esocial> Tabs4Esocial { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplica todas as IEntityTypeConfiguration<> neste assembly (Configurations/*)
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
