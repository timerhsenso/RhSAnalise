// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/GrauInstrucaoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class GrauInstrucaoConfiguration : IEntityTypeConfiguration<GrauInstrucao>
    {
        public void Configure(EntityTypeBuilder<GrauInstrucao> builder)
        {
            builder.ToTable("tgin1");

            builder.HasKey(g => g.CodigoInstrucao);

            builder.Property(g => g.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(g => g.CodigoInstrucao)
                .HasColumnName("cdinstruc")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(g => g.DescricaoInstrucao)
                .HasColumnName("dcinstruc")
                .HasMaxLength(40)
                .IsRequired();

            builder.Property(g => g.CodigoRAIS)
                .HasColumnName("cdrais")
                .HasColumnType("char(2)");

            builder.Property(g => g.CodigoCAGED)
                .HasColumnName("cdcaged")
                .HasColumnType("char(2)");

            builder.Property(g => g.CodigoESocial)
                .HasColumnName("cdesocial")
                .HasColumnType("char(2)");

            // Índice
            builder.HasIndex(g => g.CodigoInstrucao)
                .HasDatabaseName("tgin1nx1")
                .IsUnique();
        }
    }
}