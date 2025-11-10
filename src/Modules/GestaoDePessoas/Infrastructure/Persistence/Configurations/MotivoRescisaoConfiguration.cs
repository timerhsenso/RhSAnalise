// src/Modules/GestaoDePessoas/Infrastructure/Persistence/Configurations/MotivoRescisaoConfiguration.cs

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations
{
    public class MotivoRescisaoConfiguration : IEntityTypeConfiguration<MotivoRescisao>
    {
        public void Configure(EntityTypeBuilder<MotivoRescisao> builder)
        {
            builder.ToTable("tcre1");

            builder.HasKey(m => m.CodigoRescisao);

            builder.Property(m => m.Id)
                .HasColumnName("id")
                .HasDefaultValueSql("newsequentialid()");

            builder.Property(m => m.CodigoRescisao)
                .HasColumnName("cdrescisao")
                .HasColumnType("char(2)")
                .IsRequired();

            builder.Property(m => m.DescricaoRescisao)
                .HasColumnName("dcrescisao")
                .HasMaxLength(120);

            builder.Property(m => m.CodigoFGTS)
                .HasColumnName("cdfgts")
                .HasColumnType("char(1)");

            builder.Property(m => m.ComAviso)
                .HasColumnName("chaviso")
                .HasColumnType("char(1)");

            builder.Property(m => m.CodigoRAIS)
                .HasColumnName("cdrais")
                .HasColumnType("char(2)");

            builder.Property(m => m.DescricaoReduzida)
                .HasColumnName("dcreduzida")
                .HasMaxLength(80);

            builder.Property(m => m.CodigoSaque)
                .HasColumnName("cdsaque")
                .HasColumnType("char(2)");

            builder.Property(m => m.CodigoCAGED)
                .HasColumnName("cdcaged")
                .HasColumnType("char(2)");

            builder.Property(m => m.CodigoSEFIP)
                .HasColumnName("cdsefip")
                .HasColumnType("char(2)");

            builder.Property(m => m.ComRecebimentoGRRF)
                .HasColumnName("chrecgrrf")
                .HasColumnType("char(1)");

            builder.Property(m => m.CodigoAfastamentoRCT)
                .HasColumnName("cdafastrct")
                .HasColumnType("char(5)");

            builder.Property(m => m.CodigoESocial)
                .HasColumnName("cod_esocial")
                .HasColumnType("char(2)");

            builder.Property(m => m.FlagTerminoContrato)
                .HasColumnName("fltermcontr")
                .HasColumnType("char(1)");
        }
    }
}