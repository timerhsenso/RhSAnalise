using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class TabelaSalarialConfiguration : IEntityTypeConfiguration<TabelaSalarial>
{
    public void Configure(EntityTypeBuilder<TabelaSalarial> builder)
    {
        builder.ToTable("tsal1");

        // PK inferida (o script não define explicitamente)
        builder.HasKey(x => x.CdTabela);

        builder.Property(x => x.CdTabela)
               .HasColumnName("cdtabela")
               .HasColumnType("char(3)")
               .IsRequired();

        builder.Property(x => x.DcTabela)
               .HasColumnName("dctabela")
               .HasMaxLength(220);

        builder.Property(x => x.FlSeq)
               .HasColumnName("flseq")
               .HasColumnType("char(1)");

        builder.Property(x => x.Id)
               .HasColumnName("id")
               .HasDefaultValueSql("newsequentialid()");

        builder.Property(x => x.VlSalInicial)
               .HasColumnName("vlsalinicial")
               .HasColumnType("numeric(12,2)");

        builder.Property(x => x.VlSalMediana)
               .HasColumnName("vlsalmediana")
               .HasColumnType("numeric(12,2)");

        builder.Property(x => x.VlSalMaximo)
               .HasColumnName("vlsalmaximo")
               .HasColumnType("numeric(12,2)");

        builder.Property(x => x.DtValidade)
               .HasColumnName("dtvalidade");

        builder.Property(x => x.IdTsalValidade)
               .HasColumnName("idtsalvalidade");

        builder.Property(x => x.TsalValidadeId)
               .HasColumnName("tsalvalidade_id")
               .HasColumnType("numeric(10,0)");
    }
}
