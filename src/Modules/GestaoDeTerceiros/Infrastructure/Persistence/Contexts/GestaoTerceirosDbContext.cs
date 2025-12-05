using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Infrastructure.Persistence.Contexts;

/// <summary>
/// DbContext do módulo Gestão de Terceiros (GTR)
/// </summary>
public class GestaoTerceirosDbContext : DbContext
{
    public GestaoTerceirosDbContext(DbContextOptions<GestaoTerceirosDbContext> options)
        : base(options)
    {
    }

    // ═══════════════════════════════════════════════════════════════════
    // Tabelas Auxiliares (Lookups)
    // ═══════════════════════════════════════════════════════════════════

    public DbSet<TipoFornecedor> TiposFornecedor { get; set; } = null!;
    public DbSet<TipoVeiculo> TiposVeiculo { get; set; } = null!;
    public DbSet<TipoDocumentoTerceiro> TiposDocumentoTerceiro { get; set; } = null!;
    public DbSet<TipoDocumentoVeiculo> TiposDocumentoVeiculo { get; set; } = null!;
    public DbSet<TipoTreinamento> TiposTreinamento { get; set; } = null!;
    public DbSet<TipoAso> TiposAso { get; set; } = null!;
    public DbSet<TipoParentesco> TiposParentesco { get; set; } = null!;
    public DbSet<TipoSanguineo> TiposSanguineo { get; set; } = null!;
    public DbSet<TipoContato> TiposContato { get; set; } = null!;

    // ═══════════════════════════════════════════════════════════════════
    // Tabelas de Cadastro
    // ═══════════════════════════════════════════════════════════════════

    public DbSet<FornecedorEmpresa> FornecedoresEmpresas { get; set; } = null!;
    public DbSet<FornecedorColaborador> FornecedoresColaboradores { get; set; } = null!;
    public DbSet<FornecedorContrato> FornecedoresContratos { get; set; } = null!;
    public DbSet<FornecedorContratoServico> FornecedoresContratosServicos { get; set; } = null!;
    public DbSet<PessoaDocumento> PessoasDocumentos { get; set; } = null!;
    public DbSet<PessoaContato> PessoasContatos { get; set; } = null!;
    public DbSet<Veiculo> Veiculos { get; set; } = null!;
    public DbSet<VeiculoDocumento> VeiculosDocumentos { get; set; } = null!;
    public DbSet<TreinamentoTurma> TreinamentosTurmas { get; set; } = null!;
    public DbSet<TreinamentoParticipante> TreinamentosParticipantes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplica todas as configurações do assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configurações de índices
        ConfigurarIndices(modelBuilder);
    }

    private void ConfigurarIndices(ModelBuilder modelBuilder)
    {
        // ═══════════════════════════════════════════════════════════════════
        // Índices para tabelas auxiliares (código único por tenant)
        // ═══════════════════════════════════════════════════════════════════

        modelBuilder.Entity<TipoFornecedor>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoVeiculo>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoDocumentoTerceiro>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoTreinamento>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoAso>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoParentesco>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoSanguineo>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoContato>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        modelBuilder.Entity<TipoDocumentoVeiculo>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Codigo }).IsUnique().HasFilter("[Ativo] = 1");
        });

        // ═══════════════════════════════════════════════════════════════════
        // Índices para tabelas de cadastro
        // ═══════════════════════════════════════════════════════════════════

        modelBuilder.Entity<FornecedorEmpresa>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.CNPJ }).IsUnique().HasFilter("[Ativo] = 1");
            entity.HasIndex(e => e.RazaoSocial);
        });

        modelBuilder.Entity<FornecedorColaborador>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.CPF }).IsUnique().HasFilter("[Ativo] = 1");
            entity.HasIndex(e => e.Nome);
            entity.HasIndex(e => e.IdFornecedorEmpresa);
        });

        modelBuilder.Entity<FornecedorContrato>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.NumeroContrato }).IsUnique().HasFilter("[Ativo] = 1");
            entity.HasIndex(e => e.IdFornecedorEmpresa);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<PessoaDocumento>(entity =>
        {
            entity.HasIndex(e => e.IdFornecedorColaborador);
            entity.HasIndex(e => e.IdTipoDocumentoTerceiro);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DataValidade);
        });

        modelBuilder.Entity<Veiculo>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.Placa }).IsUnique().HasFilter("[Ativo] = 1");
            entity.HasIndex(e => e.IdFornecedorEmpresa);
        });

        modelBuilder.Entity<VeiculoDocumento>(entity =>
        {
            entity.HasIndex(e => e.IdVeiculo);
            entity.HasIndex(e => e.IdTipoDocumentoVeiculo);
            entity.HasIndex(e => e.DataValidade);
        });

        modelBuilder.Entity<TreinamentoTurma>(entity =>
        {
            entity.HasIndex(e => new { e.IdSaas, e.CodigoTurma }).IsUnique().HasFilter("[Ativo] = 1");
            entity.HasIndex(e => e.DataInicio);
            entity.HasIndex(e => e.Status);
        });

        modelBuilder.Entity<TreinamentoParticipante>(entity =>
        {
            entity.HasIndex(e => new { e.IdTreinamentoTurma, e.IdFornecedorColaborador }).IsUnique();
            entity.HasIndex(e => e.DataValidadeCertificado);
        });
    }
}
