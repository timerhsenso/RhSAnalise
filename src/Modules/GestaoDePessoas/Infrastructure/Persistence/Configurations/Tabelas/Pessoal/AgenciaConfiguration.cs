// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/AgenciaConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal
{
    public class AgenciaConfiguration : IEntityTypeConfiguration<Agencia>
    {
        public void Configure(EntityTypeBuilder<Agencia> builder)
        {
            builder.ToTable("tage1");

            // Chave Primária Composta
            builder.HasKey(a => new { a.CodigoBanco, a.CodigoAgencia });

            builder.Property(a => a.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(a => a.CodigoBanco)
                .HasColumnName("cdbanco")
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(a => a.CodigoAgencia)
                .HasColumnName("cdagencia")
                .HasColumnType("char(4)")
                .IsRequired();

            builder.Property(a => a.DigitoVerificador)
                .HasColumnName("dvagencia")
                .HasColumnType("char(1)");

            builder.Property(a => a.NomeAgencia)
                .HasColumnName("nmagencia")
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(a => a.CodigoMunicipio)
                .HasColumnName("cdmunicip")
                .HasColumnType("char(5)");

            builder.Property(a => a.NumeroConta)
                .HasColumnName("noconta")
                .HasColumnType("char(15)");

            builder.Property(a => a.IdBanco)
                .HasColumnName("idbanco");

            // Índice
            builder.HasIndex(a => new { a.CodigoBanco, a.CodigoAgencia })
                .HasDatabaseName("tage1nx1");

            // Relacionamento
            builder.HasOne(a => a.Banco)
                .WithMany(b => b.Agencias)
                .HasForeignKey(a => a.IdBanco)
                .HasConstraintName("FK_tage1_tban1_idbanco");
        }
    }
}