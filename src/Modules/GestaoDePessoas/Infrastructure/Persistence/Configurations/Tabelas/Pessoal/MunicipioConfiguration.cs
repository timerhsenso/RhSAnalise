// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/MunicipioConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities.Tabelas.Pessoal;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations.Tabelas.Pessoal
{
    public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
    {
        public void Configure(EntityTypeBuilder<Municipio> builder)
        {
            builder.ToTable("muni1");

            builder.HasKey(m => m.CodigoMunicipio);

            builder.Property(m => m.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(m => m.CodigoMunicipio)
                .HasColumnName("cdmunicip")
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(m => m.SiglaEstado)
                .HasColumnName("sgestado")
                .HasColumnType("char(2)");

            builder.Property(m => m.NomeMunicipio)
                .HasColumnName("nmmunicip")
                .HasMaxLength(60)
                .IsRequired();

            builder.Property(m => m.CodigoIBGE)
                .HasColumnName("cod_ibge");

            // Índices
            builder.HasIndex(m => m.CodigoMunicipio)
                .HasDatabaseName("PK__muni1__693CA210")
                .IsUnique();

            builder.HasIndex(m => new { m.SiglaEstado, m.NomeMunicipio })
                .HasDatabaseName("UX_muni1_sgestado_nmmunicip")
                .IsUnique();
        }
    }
}