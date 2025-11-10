using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class MotivoOcorrenciaFrequenciaConfiguration : IEntityTypeConfiguration<MotivoOcorrenciaFrequencia>
{
    public void Configure(EntityTypeBuilder<MotivoOcorrenciaFrequencia> builder)
    {
        builder.ToTable("mfre1");

        builder.HasKey(x => new { x.TpOcorr, x.CdMotoc });

        builder.Property(x => x.TpOcorr)
               .HasColumnName("tpocorr")
               .IsRequired();

        builder.Property(x => x.CdMotoc)
               .HasColumnName("cdmotoc")
               .HasColumnType("char(4)")
               .IsRequired();

        builder.Property(x => x.DcMotoc)
               .HasColumnName("dcmotoc")
               .HasMaxLength(40);

        builder.Property(x => x.FlMovimen).HasColumnName("flmovimen");
        builder.Property(x => x.CdConta).HasColumnName("cdconta").HasColumnType("char(4)");
        builder.Property(x => x.FlTpFal).HasColumnName("fltpfal");
        builder.Property(x => x.FlExtra).HasColumnName("flextra");
        builder.Property(x => x.FlFlAnj).HasColumnName("flflanj");
        builder.Property(x => x.FlTroca).HasColumnName("FLTROCA");
        builder.Property(x => x.FlRegraHe).HasColumnName("FLREGRAHE");

        builder.Property(x => x.FlBancoHoras)
               .HasColumnName("FLBANCOHORAS")
               .IsRequired();

        builder.Property(x => x.TpOcorrLink).HasColumnName("TPOCORRLINK");
        builder.Property(x => x.CdMotocLink).HasColumnName("CDMOTOCLINK").HasColumnType("char(4)");

        builder.Property(x => x.Id)
               .HasColumnName("id")
               .HasDefaultValueSql("newid()");

        builder.Property(x => x.IdMotivosDeOcorrenciaFrequenciaPai)
               .HasColumnName("idmotivosdeocorrenciafrequenciapai");

        builder.Property(x => x.IdVerba)
               .HasColumnName("idverba");
    }
}
