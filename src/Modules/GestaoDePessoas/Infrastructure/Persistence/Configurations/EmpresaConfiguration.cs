// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/EmpresaConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class EmpresaConfiguration : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("temp1");

            builder.HasKey(e => e.CodigoEmpresa);

            builder.Property(e => e.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(e => e.CodigoEmpresa)
                .HasColumnName("cdempresa")
                .IsRequired();

            builder.Property(e => e.NomeEmpresa)
                .HasColumnName("nmempresa")
                .HasMaxLength(100);

            builder.Property(e => e.NomeFantasia)
                .HasColumnName("nmfantasia")
                .HasMaxLength(30);

            builder.Property(e => e.TipoCheque)
                .HasColumnName("chtpcche")
                .HasColumnType("char(2)");

            builder.Property(e => e.TipoDARF)
                .HasColumnName("chtpdarf")
                .HasColumnType("char(2)");

            builder.Property(e => e.TipoGRPS)
                .HasColumnName("chtpgrps")
                .HasColumnType("char(2)");

            builder.Property(e => e.TipoRescisao)
                .HasColumnName("chtptres")
                .HasColumnType("char(2)");

            builder.Property(e => e.BrowseFuncionario)
                .HasColumnName("chbrwfunc")
                .HasColumnType("char(1)");

            builder.Property(e => e.TotalOrcamento)
                .HasColumnName("chtorc1")
                .HasColumnType("char(1)");

            builder.Property(e => e.CalculoFerias)
                .HasColumnName("chferias")
                .HasColumnType("char(1)");

            builder.Property(e => e.ArquivoLogo)
                .HasColumnName("nmarqlogo")
                .HasColumnType("char(80)");

            builder.Property(e => e.ArquivoLogoCracha)
                .HasColumnName("nmarqlogocra")
                .HasMaxLength(80);

            builder.Property(e => e.FlagFolhaAposentadoriaSocial)
                .HasColumnName("flfapesocial");

            builder.Property(e => e.TipoInscricaoEmpregador)
                .HasColumnName("tpinscempregador")
                .HasColumnType("char(1)");

            builder.Property(e => e.NumeroInscricaoEmpregador)
                .HasColumnName("nrinscempregador")
                .HasMaxLength(15);

            builder.Property(e => e.FlagAtivo)
                .HasColumnName("flativo")
                .HasColumnType("char(1)");

            builder.Property(e => e.Logo)
                .HasColumnName("logo")
                .HasColumnType("image");

            builder.Property(e => e.LogoCracha)
                .HasColumnName("logocracha")
                .HasColumnType("image");

            builder.Property(e => e.ClassificacaoTributaria)
                .HasColumnName("classtrib")
                .HasColumnType("char(2)");

            builder.Property(e => e.CNPJEmpresaFiscalResponsavel)
                .HasColumnName("cnpjefr")
                .HasColumnType("char(14)");

            builder.Property(e => e.DataDOU)
                .HasColumnName("dtdou");

            builder.Property(e => e.DataEmissaoCertificado)
                .HasColumnName("dtemissaocertificado");

            builder.Property(e => e.DataProtocoloRenovacao)
                .HasColumnName("dtprotrenovacao");

            builder.Property(e => e.DataVencimentoCertificado)
                .HasColumnName("dtvenctocertificado");

            builder.Property(e => e.IdentificadorMinimoLei)
                .HasColumnName("ideminlei")
                .HasMaxLength(70);

            builder.Property(e => e.IndicadorAcordoIsencaoMulta)
                .HasColumnName("indacordoisenmulta");

            builder.Property(e => e.IndicadorConstrutora)
                .HasColumnName("indconstrutora");

            builder.Property(e => e.IndicadorCooperativa)
                .HasColumnName("indcooperativa");

            builder.Property(e => e.IndicadorDesoneracaoFolha)
                .HasColumnName("inddesfolha");

            builder.Property(e => e.IndicadorOpcaoCCP)
                .HasColumnName("indopccp");

            builder.Property(e => e.IndicadorPorteEmpresa)
                .HasColumnName("indporte")
                .HasColumnType("char(1)");

            builder.Property(e => e.IndicadorOptoRegistroEletronico)
                .HasColumnName("indoptregeletronico");

            builder.Property(e => e.NaturezaJuridica)
                .HasColumnName("natjuridica")
                .HasColumnType("char(4)");

            builder.Property(e => e.NumeroCertificado)
                .HasColumnName("nrcertificado")
                .HasMaxLength(40);

            builder.Property(e => e.NumeroProtocoloRenovacao)
                .HasColumnName("nrprotrenovacao")
                .HasMaxLength(40);

            builder.Property(e => e.NumeroRegistroETT)
                .HasColumnName("nrregett")
                .HasMaxLength(30);

            builder.Property(e => e.PaginaDOU)
                .HasColumnName("paginadou")
                .HasMaxLength(5);

            // Índice
            builder.HasIndex(e => e.CodigoEmpresa)
                .HasDatabaseName("temp1nx1");

            // Relacionamentos
            builder.HasMany(e => e.Filiais)
                .WithOne(f => f.Empresa)
                .HasForeignKey(f => f.CodigoEmpresa);

            builder.HasMany(e => e.Funcionarios)
                .WithOne(f => f.Empresa)
                .HasForeignKey(f => f.CodigoEmpresa);
        }
    }
}