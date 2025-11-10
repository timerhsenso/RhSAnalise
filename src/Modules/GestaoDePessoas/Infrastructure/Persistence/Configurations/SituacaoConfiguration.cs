// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/SituacaoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class SituacaoConfiguration : IEntityTypeConfiguration<Situacao>
    {
        public void Configure(EntityTypeBuilder<Situacao> builder)
        {
            builder.ToTable("tsitu1");

            builder.HasKey(s => s.CodigoSituacao);

            builder.Property(s => s.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(s => s.CodigoSituacao)
                .HasColumnName("cdsituacao")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(s => s.DescricaoSituacao)
                .HasColumnName("dcsituacao")
                .HasMaxLength(40);

            builder.Property(s => s.FlagDemissao)
                .HasColumnName("fldemissao")
                .HasColumnType("char(1)");

            builder.Property(s => s.FlagAfastamento)
                .HasColumnName("flafastame")
                .HasColumnType("char(1)");

            builder.Property(s => s.QuantidadeDiasBeneficio)
                .HasColumnName("qtdiasbene");

            builder.Property(s => s.QuantidadeDiasPrevistos)
                .HasColumnName("qtdiasprev");

            builder.Property(s => s.CodigoFGTS)
                .HasColumnName("cdfgts")
                .HasColumnType("char(1)");

            builder.Property(s => s.CodigoSEFIP)
                .HasColumnName("cdsefip")
                .HasColumnType("char(1)");

            builder.Property(s => s.CodigoSEFIP2)
                .HasColumnName("cdsefip2")
                .HasColumnType("char(2)");

            builder.Property(s => s.FlagPagamentoFerias)
                .HasColumnName("flpferias")
                .HasColumnType("char(1)");

            // Índice
            builder.HasIndex(s => s.CodigoSituacao)
                .HasDatabaseName("TSITU1NX1");
        }
    }
}