using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;
using System.Reflection;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Contexts
{
    public class ControleDePontoDbContext : DbContext
    {
        public ControleDePontoDbContext(DbContextOptions<ControleDePontoDbContext> options)
            : base(options)
        {
        }

        public DbSet<BancoHoras> BancosHoras { get; set; }
        public DbSet<Batidas> Batidas { get; set; }
        public DbSet<Calnd1> Calendarios { get; set; }
        public DbSet<Chor1> HorariosAdministrativos { get; set; }
        public DbSet<Chor2> VariacoesHorariosPorDia { get; set; }
        public DbSet<Ctur1> EscalasPorTurma { get; set; }
        public DbSet<Freq1> OcorrenciasFrequencia { get; set; }
        public DbSet<Freq2> PeriodosFrequencia { get; set; }
        public DbSet<Freq4> ConsolidacaoDiaria { get; set; }
        public DbSet<Hjor1> HistoricoJornada { get; set; }
        public DbSet<Mfre1> MotivosOcorrencia { get; set; }
        public DbSet<Mfre2> MotivosOcorrenciaLocal { get; set; }
        public DbSet<Sitc2> SituacaoFechamento { get; set; }
        public DbSet<Test1Freq> ParametrosCalculoFrequencia { get; set; }
        public DbSet<Turm1> Turmas { get; set; }
        public DbSet<Vprh1> ValoresParametrizadosRH { get; set; }
        public DbSet<Vprh2> ValoresParametrizadosEfetivos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
