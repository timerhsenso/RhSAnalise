using Microsoft.EntityFrameworkCore;
using System.Reflection;
using RhSensoERP.Modules.GestaoDeTerceiros.Core.Entities;

namespace RhSensoERP.Modules.GestaoDeTerceiros.Infrastructure.Persistence.Contexts;

public class GestaoDeTerceirosDbContext : DbContext
{
    public GestaoDeTerceirosDbContext(DbContextOptions<GestaoDeTerceirosDbContext> options)
        : base(options)
    {
    }

    // Auxiliares
    public DbSet<TipoFornecedor> TiposFornecedor { get; set; } = null!;
    public DbSet<TipoVeiculo> TiposVeiculo { get; set; } = null!;
    public DbSet<TipoDocumentoTerceiro> TiposDocumentoTerceiro { get; set; } = null!;
    public DbSet<TipoDocumentoVeiculo> TiposDocumentoVeiculo { get; set; } = null!;
    public DbSet<TipoTreinamento> TiposTreinamento { get; set; } = null!;
    public DbSet<TipoAso> TiposAso { get; set; } = null!;
    public DbSet<TipoParentesco> TiposParentesco { get; set; } = null!;
    public DbSet<TipoSanguineo> TiposSanguineo { get; set; } = null!;
    public DbSet<TipoContato> TiposContato { get; set; } = null!;

    // Cadastrais
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
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        ConfigurarIndices(modelBuilder);
    }

    private void ConfigurarIndices(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoFornecedor>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoVeiculo>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoDocumentoTerceiro>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoDocumentoVeiculo>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoTreinamento>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoAso>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoParentesco>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoSanguineo>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));
        modelBuilder.Entity<TipoContato>(e => e.HasIndex(x => new { x.IdSaas, x.Codigo }).IsUnique().HasFilter("[Ativo] = 1"));

        modelBuilder.Entity<FornecedorEmpresa>(e => { e.HasIndex(x => new { x.IdSaas, x.CNPJ }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.RazaoSocial); });
        modelBuilder.Entity<FornecedorColaborador>(e => { e.HasIndex(x => new { x.IdSaas, x.CPF }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.Nome); e.HasIndex(x => x.IdFornecedorEmpresa); });
        modelBuilder.Entity<FornecedorContrato>(e => { e.HasIndex(x => new { x.IdSaas, x.NumeroContrato }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.IdFornecedorEmpresa); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<PessoaDocumento>(e => { e.HasIndex(x => x.IdFornecedorColaborador); e.HasIndex(x => x.IdTipoDocumentoTerceiro); e.HasIndex(x => x.Status); e.HasIndex(x => x.DataValidade); });
        modelBuilder.Entity<Veiculo>(e => { e.HasIndex(x => new { x.IdSaas, x.Placa }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.IdFornecedorEmpresa); });
        modelBuilder.Entity<VeiculoDocumento>(e => { e.HasIndex(x => x.IdVeiculo); e.HasIndex(x => x.IdTipoDocumentoVeiculo); e.HasIndex(x => x.DataValidade); });
        modelBuilder.Entity<TreinamentoTurma>(e => { e.HasIndex(x => new { x.IdSaas, x.CodigoTurma }).IsUnique().HasFilter("[Ativo] = 1"); e.HasIndex(x => x.DataInicio); e.HasIndex(x => x.Status); });
        modelBuilder.Entity<TreinamentoParticipante>(e => { e.HasIndex(x => new { x.IdTreinamentoTurma, x.IdFornecedorColaborador }).IsUnique(); e.HasIndex(x => x.DataValidadeCertificado); });
    }
}
