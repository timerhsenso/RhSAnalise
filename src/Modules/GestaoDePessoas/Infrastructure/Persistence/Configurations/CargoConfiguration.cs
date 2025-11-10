// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/CargoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class CargoConfiguration : IEntityTypeConfiguration<Cargo>
    {
        public void Configure(EntityTypeBuilder<Cargo> builder)
        {
            builder.ToTable("cargo1");

            builder.HasKey(c => c.CodigoCargo);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(c => c.CodigoCargo)
                .HasColumnName("cdcargo")
                .HasColumnType("char(5)")
                .IsRequired();

            builder.Property(c => c.DescricaoCargo)
                .HasColumnName("dccargo")
                .HasMaxLength(50);

            builder.Property(c => c.CodigoInstrucao)
                .HasColumnName("cdinstruc")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(c => c.CodigoCBO)
                .HasColumnName("cdcbo")
                .HasColumnType("char(5)");

            builder.Property(c => c.CodigoTabela)
                .HasColumnName("cdtabela")
                .HasColumnType("char(3)");

            builder.Property(c => c.CodigoNivelInicial)
                .HasColumnName("cdniveini")
                .HasColumnType("char(5)");

            builder.Property(c => c.CodigoNivelFinal)
                .HasColumnName("cdnivefim")
                .HasColumnType("char(5)");

            builder.Property(c => c.FlagAtivo)
                .HasColumnName("flativo")
                .IsRequired();

            builder.Property(c => c.CodigoGrupoProfissional)
                .HasColumnName("cdgrprof")
                .HasColumnType("char(2)");

            builder.Property(c => c.CodigoCBO6)
                .HasColumnName("cdcbo6")
                .HasColumnType("char(6)");

            builder.Property(c => c.DataInicioValidade)
                .HasColumnName("dtinival");

            builder.Property(c => c.DataFimValidade)
                .HasColumnName("dtfimval");

            builder.Property(c => c.Tenant)
                .HasColumnName("Tenant")
                .HasDefaultValue(0);

            builder.Property(c => c.IdCBO)
                .HasColumnName("idcbo");

            builder.Property(c => c.IdGrauInstrucao)
                .HasColumnName("idgraudeinstrucao");

            builder.Property(c => c.IdTabelaSalarial)
                .HasColumnName("idtabelasalarial");

            // Índices
            builder.HasIndex(c => c.CodigoCargo)
                .HasDatabaseName("cargo1nx1")
                .IsUnique();

            builder.HasIndex(c => new { c.CodigoCargo, c.CodigoTabela })
                .HasDatabaseName("cargo1nx2");

            // Relacionamentos
            builder.HasOne(c => c.GrauInstrucao)
                .WithMany(gi => gi.Cargos)
                .HasForeignKey(c => c.IdGrauInstrucao)
                .HasConstraintName("FK_cargo1_tgin1_idgraudeinstrucao");
        }
    }
}