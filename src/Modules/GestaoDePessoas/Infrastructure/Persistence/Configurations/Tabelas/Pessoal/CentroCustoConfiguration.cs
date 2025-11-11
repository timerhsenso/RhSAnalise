// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/CentroCustoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal
{
    public class CentroCustoConfiguration : IEntityTypeConfiguration<CentroCusto>
    {
        public void Configure(EntityTypeBuilder<CentroCusto> builder)
        {
            builder.ToTable("tcus1");

            builder.HasKey(c => c.CodigoCentroCusto);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(c => c.CodigoCentroCusto)
                .HasColumnName("cdccusto")
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(c => c.DescricaoCentroCusto)
                .HasColumnName("dcccusto")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.SiglaCentroCusto)
                .HasColumnName("sgccusto")
                .HasMaxLength(20);

            builder.Property(c => c.NumeroCentroCusto)
                .HasColumnName("noccusto")
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.FlagAtivo)
                .HasColumnName("flativo");

            builder.Property(c => c.DescricaoAreaCracha)
                .HasColumnName("dcarea_cracha")
                .HasMaxLength(25);

            builder.Property(c => c.DataBloqueio)
                .HasColumnName("dtbloqueio");

            builder.Property(c => c.CodigoCentroCustoPai)
                .HasColumnName("cdccusto_pai")
                .HasColumnType("char(5)");

            builder.Property(c => c.CodigoCentroCustoResponsavel)
                .HasColumnName("cdccresp")
                .HasColumnType("char(20)");

            builder.Property(c => c.FlagCentroCusto)
                .HasColumnName("flccusto")
                .HasColumnType("char(1)");

            // Índices
            builder.HasIndex(c => c.CodigoCentroCusto)
                .HasDatabaseName("tcus1nx1");

            builder.HasIndex(c => c.NumeroCentroCusto)
                .HasDatabaseName("IX_tcus1_noccusto");
        }
    }
}