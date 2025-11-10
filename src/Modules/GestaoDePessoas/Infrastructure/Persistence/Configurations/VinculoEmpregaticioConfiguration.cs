// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/VinculoEmpregaticioConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class VinculoEmpregaticioConfiguration : IEntityTypeConfiguration<VinculoEmpregaticio>
    {
        public void Configure(EntityTypeBuilder<VinculoEmpregaticio> builder)
        {
            builder.ToTable("tvin1");

            builder.HasKey(v => v.CodigoVinculo);

            builder.Property(v => v.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(v => v.CodigoVinculo)
                .HasColumnName("cdvincul")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(v => v.DescricaoVinculo)
                .HasColumnName("dcvincul")
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(v => v.CodigoSEFIP)
                .HasColumnName("cdsefip")
                .HasColumnType("char(2)");

            builder.Property(v => v.CodigoClasse)
                .HasColumnName("cdclasse")
                .HasColumnType("char(2)");

            builder.Property(v => v.FlagRAIS)
                .HasColumnName("flrais")
                .IsRequired();

            builder.Property(v => v.NaturezaAtividade)
                .HasColumnName("natatividade")
                .IsRequired();
        }
    }
}