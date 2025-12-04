using Microsoft.EntityFrameworkCore;
using RhSensoERP.Modules.Esocial.Core.Entities;

namespace RhSensoERP.Modules.Esocial.Infrastructure.Persistence.Contexts;

public class EsocialDbContext : DbContext
{
    public EsocialDbContext(DbContextOptions<EsocialDbContext> options) : base(options)
    {
    }

    public DbSet<EsocialTabela4> Tabela4 => Set<EsocialTabela4>();
    public DbSet<EsocialTabela8> Tabela8 => Set<EsocialTabela8>();
    public DbSet<EsocialTabela10> Tabela10 => Set<EsocialTabela10>();
    public DbSet<EsocialTabela21> Tabela21 => Set<EsocialTabela21>();
    public DbSet<LotacaoTributaria> LotacoesTributarias => Set<LotacaoTributaria>();
    public DbSet<EsocialMotivoOcorrencia> MotivosOcorrencia => Set<EsocialMotivoOcorrencia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ═══════════════════════════════════════════════════════════════════
        // Tabela 4 - FPAS
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<EsocialTabela4>(entity =>
        {
            entity.ToTable("tab4_esocial");
            entity.HasKey(e => e.Tab4Codigo);

            entity.Property(e => e.Tab4Codigo)
                .HasColumnName("tab4_codigo")
                .HasMaxLength(3)
                .IsRequired();

            entity.Property(e => e.Tab4Descricao)
                .HasColumnName("tab4_descricao")
                .HasMaxLength(255);
        });

        // ═══════════════════════════════════════════════════════════════════
        // Tabela 8 - Classificação Tributária
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<EsocialTabela8>(entity =>
        {
            entity.ToTable("tab8_esocial");
            entity.HasKey(e => e.Tab8Codigo);

            entity.Property(e => e.Tab8Codigo)
                .HasColumnName("tab8_codigo")
                .HasMaxLength(2)
                .IsRequired();

            entity.Property(e => e.Tab8Descricao)
                .HasColumnName("tab8_descricao")
                .HasMaxLength(255);
        });

        // ═══════════════════════════════════════════════════════════════════
        // Tabela 10 - Tipos de Lotação Tributária
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<EsocialTabela10>(entity =>
        {
            entity.ToTable("tab10_esocial");
            entity.HasKey(e => e.Tab10Codigo);

            entity.Property(e => e.Tab10Codigo)
                .HasColumnName("tab10_codigo")
                .HasMaxLength(2)
                .IsRequired();

            entity.Property(e => e.Tab10Descricao)
                .HasColumnName("tab10_descricao")
                .HasMaxLength(255);

            entity.Property(e => e.Tab10DescDocRequisito)
                .HasColumnName("tab10_desc_doc_requisito")
                .HasMaxLength(255);
        });

        // ═══════════════════════════════════════════════════════════════════
        // Tabela 21 - Natureza Jurídica
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<EsocialTabela21>(entity =>
        {
            entity.ToTable("tab21_esocial");
            entity.HasKey(e => e.Tab21Codigo);

            entity.Property(e => e.Tab21Codigo)
                .HasColumnName("tab21_codigo")
                .HasMaxLength(4)
                .IsRequired();

            entity.Property(e => e.Tab21Descricao)
                .HasColumnName("tab21_descricao")
                .HasMaxLength(255);
        });

        // ═══════════════════════════════════════════════════════════════════
        // Lotação Tributária
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<LotacaoTributaria>(entity =>
        {
            entity.ToTable("lotacoestributarias");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.CodLotacao)
                .HasColumnName("codlotacao")
                .HasMaxLength(30);

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(255);

            entity.Property(e => e.TpLotacao)
                .HasColumnName("tplotacao")
                .HasMaxLength(2);

            entity.Property(e => e.Fpas)
                .HasColumnName("fpas")
                .HasMaxLength(3);

            // Relacionamento com Tabela 10
            entity.HasOne(e => e.Tabela10)
                .WithMany(t => t.Lotacoes)
                .HasForeignKey(e => e.TpLotacao)
                .HasPrincipalKey(t => t.Tab10Codigo)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Tabela 4
            entity.HasOne(e => e.Tabela4)
                .WithMany(t => t.Lotacoes)
                .HasForeignKey(e => e.Fpas)
                .HasPrincipalKey(t => t.Tab4Codigo)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ═══════════════════════════════════════════════════════════════════
        // Motivo de Ocorrência
        // PK Composta: (TpOcorr, CdMotoc) - conforme banco
        // Id é chave alternativa única
        // ═══════════════════════════════════════════════════════════════════
        modelBuilder.Entity<EsocialMotivoOcorrencia>(entity =>
        {
            entity.ToTable("mfre1");

            // PK COMPOSTA (conforme DDL do banco)
            entity.HasKey(e => new { e.TpOcorr, e.CdMotoc });

            // Chave alternativa única (id tem UNIQUE no banco)
            entity.HasAlternateKey(e => e.Id);

            // Propriedades
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("NEWID()");

            entity.Property(e => e.TpOcorr)
                .HasColumnName("tpocorr")
                .IsRequired();

            entity.Property(e => e.CdMotoc)
                .HasColumnName("cdmotoc")
                .HasColumnType("char(4)")
                .HasMaxLength(4)
                .IsRequired();

            entity.Property(e => e.DcMotoc)
                .HasColumnName("dcmotoc")
                .HasMaxLength(40);

            entity.Property(e => e.FlMovimen)
                .HasColumnName("flmovimen");

            entity.Property(e => e.CdConta)
                .HasColumnName("cdconta")
                .HasColumnType("char(4)")
                .HasMaxLength(4);

            entity.Property(e => e.FlTpFal)
                .HasColumnName("fltpfal");

            entity.Property(e => e.FlExtra)
                .HasColumnName("flextra");

            entity.Property(e => e.FlFlAnj)
                .HasColumnName("flflanj");

            entity.Property(e => e.FlTroca)
                .HasColumnName("FLTROCA");

            entity.Property(e => e.FlRegraHE)
                .HasColumnName("FLREGRAHE");

            entity.Property(e => e.FlBancoHoras)
                .HasColumnName("FLBANCOHORAS")
                .IsRequired();

            entity.Property(e => e.TpOcorrLink)
                .HasColumnName("TPOCORRLINK");

            entity.Property(e => e.CdMotocLink)
                .HasColumnName("CDMOTOCLINK")
                .HasColumnType("char(4)")
                .HasMaxLength(4);

            entity.Property(e => e.IdMotivoPai)
                .HasColumnName("idmotivosdeocorrenciafrequenciapai");

            entity.Property(e => e.IdVerba)
                .HasColumnName("idverba");

            // Auto-referência para motivo pai (usando Id como FK)
            entity.HasOne(e => e.MotivoPai)
                .WithMany()
                .HasForeignKey(e => e.IdMotivoPai)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}