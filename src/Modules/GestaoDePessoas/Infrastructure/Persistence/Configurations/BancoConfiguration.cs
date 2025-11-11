// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/BancoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class BancoConfiguration : IEntityTypeConfiguration<Banco>
    {
        public void Configure(EntityTypeBuilder<Banco> builder)
        {
            builder.ToTable("tban1");

            builder.HasKey(b => b.CodigoBanco);

            builder.Property(b => b.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(b => b.CodigoBanco)
                .HasColumnName("cdbanco")
                .HasColumnType("char(3)")
                .IsRequired();

            builder.Property(b => b.DescricaoBanco)
                .HasColumnName("dcbanco")
                .HasMaxLength(40)
                .IsRequired();

            // Índice
            builder.HasIndex(b => b.CodigoBanco)
                .HasDatabaseName("tban1nx1")
                .IsUnique();
        }
    }
}