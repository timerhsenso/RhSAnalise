using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.ControleAcessoPortaria.Core.Entities;

namespace RhSensoERP.Modules.ControleAcessoPortaria.Infrastructure.Persistence.Contexts;

public class ControleAcessoPortariaDbContext : DbContext
{
    public ControleAcessoPortariaDbContext(DbContextOptions<ControleAcessoPortariaDbContext> options)
        : base(options)
    {
    }

    // Auxiliares
    public DbSet<TipoPessoa> TiposPessoa { get; set; } = null!;
    public DbSet<MotivoAcesso> MotivosAcesso { get; set; } = null!;
    public DbSet<StatusAcesso> StatusAcesso { get; set; } = null!;
    public DbSet<MotivoRecusa> MotivosRecusa { get; set; } = null!;
    public DbSet<TipoChecklist> TiposChecklist { get; set; } = null!;

    // Cadastrais
    public DbSet<Portaria> Portarias { get; set; } = null!;
    public DbSet<AreaAcesso> AreasAcesso { get; set; } = null!;
    public DbSet<Visitante> Visitantes { get; set; } = null!;
    public DbSet<CrachaProvisorio> CrachasProvisorio { get; set; } = null!;
    public DbSet<ChecklistModelo> ChecklistsModelo { get; set; } = null!;
    public DbSet<ChecklistItem> ChecklistsItens { get; set; } = null!;

    // Movimentação
    public DbSet<RegistroAcesso> RegistrosAcesso { get; set; } = null!;
    public DbSet<RegistroAcessoVeiculo> RegistrosAcessoVeiculo { get; set; } = null!;
    public DbSet<ChecklistExecucao> ChecklistsExecucao { get; set; } = null!;
    public DbSet<ChecklistExecucaoItem> ChecklistsExecucaoItens { get; set; } = null!;
    public DbSet<AgendamentoCarga> AgendamentosCarga { get; set; } = null!;
    public DbSet<RecebimentoCarga> RecebimentosCarga { get; set; } = null!;
    public DbSet<RecebimentoCargaProduto> RecebimentosCargaProdutos { get; set; } = null!;
    public DbSet<Ocorrencia> Ocorrencias { get; set; } = null!;
    public DbSet<ListaNegra> ListaNegra { get; set; } = null!;

    // Configuração
    public DbSet<ConfiguracaoEmpresa> ConfiguracoesEmpresa { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ConfigurarIndices(modelBuilder);
    }

    private void ConfigurarIndices(ModelBuilder modelBuilder)
    {
        // Auxiliares
        modelBuilder.Entity<TipoPessoa>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<MotivoAcesso>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<StatusAcesso>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<MotivoRecusa>(e => e.HasIndex(x => new { x.IdSaas, x.Tipo, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoChecklist>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));

        // Cadastrais
        modelBuilder.Entity<Portaria>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<AreaAcesso>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<Visitante>(e => { e.HasIndex(x => new { x.IdSaas, x.CPF }).HasFilter("[CPF] IS NOT NULL AND [Ativo] = 1"); e.HasIndex(x => x.Nome); });
        modelBuilder.Entity<CrachaProvisorio>(e => { e.HasIndex(x => new { x.IdSaas, x.IdPortaria, x.Numero }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<ChecklistModelo>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<ChecklistItem>(e => { e.HasIndex(x => new { x.IdChecklistModelo, x.Ordem }).IsUnique().HasFilter("[Ativo] = 1"); });

        // Movimentação
        modelBuilder.Entity<RegistroAcesso>(e => { e.HasIndex(x => new { x.IdSaas, x.NumeroProtocolo }).IsUnique(); e.HasIndex(x => x.DataHoraEntrada); e.HasIndex(x => x.IdStatusAcesso); e.HasIndex(x => x.CPFPessoa); });
        modelBuilder.Entity<RegistroAcessoVeiculo>(e => { e.HasIndex(x => x.IdRegistroAcesso); e.HasIndex(x => x.Placa); });
        modelBuilder.Entity<ChecklistExecucao>(e => { e.HasIndex(x => x.DataExecucao); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<ChecklistExecucaoItem>(e => e.HasIndex(x => new { x.IdChecklistExecucao, x.IdChecklistItem }).IsUnique());
        modelBuilder.Entity<AgendamentoCarga>(e => { e.HasIndex(x => new { x.IdSaas, x.NumeroAgendamento }).IsUnique(); e.HasIndex(x => x.DataAgendamento); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<RecebimentoCarga>(e => { e.HasIndex(x => new { x.IdSaas, x.NumeroRecebimento }).IsUnique(); e.HasIndex(x => x.DataHoraChegada); e.HasIndex(x => x.Status); e.HasIndex(x => x.NumeroNotaFiscal); });
        modelBuilder.Entity<Ocorrencia>(e => { e.HasIndex(x => new { x.IdSaas, x.NumeroOcorrencia }).IsUnique(); e.HasIndex(x => x.DataHoraOcorrencia); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<ListaNegra>(e => { e.HasIndex(x => x.CPF).HasFilter("[CPF] IS NOT NULL AND [Ativo] = 1"); e.HasIndex(x => x.CNPJ).HasFilter("[CNPJ] IS NOT NULL AND [Ativo] = 1"); e.HasIndex(x => x.PlacaVeiculo).HasFilter("[PlacaVeiculo] IS NOT NULL AND [Ativo] = 1"); });

        // Configuração
        modelBuilder.Entity<ConfiguracaoEmpresa>(e => e.HasIndex(x => x.IdSaas).IsUnique().HasFilter("[Ativo] = 1"));
    }
}
