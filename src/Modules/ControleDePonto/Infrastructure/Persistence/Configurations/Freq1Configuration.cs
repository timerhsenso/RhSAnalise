// Freq1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class Freq1Configuration : IEntityTypeConfiguration<Freq1>
{
    public void Configure(EntityTypeBuilder<Freq1> b)
    {
        b.ToTable("freq1");

        b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.DtOcorr, x.HhIniOcor, x.TpOcorr });

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();
        b.Property(x => x.TxOcorr).HasMaxLength(80);
        b.Property(x => x.CdCcUsRes).HasMaxLength(5);
        b.Property(x => x.CdUsuario).HasMaxLength(20);
        b.Property(x => x.NoMatTroc).HasMaxLength(8);
        b.Property(x => x.CdUsAprHe).HasMaxLength(20);
        b.Property(x => x.NoProcesso).HasMaxLength(6);
        b.Property(x => x.HhIniOcorOld).HasMaxLength(4);
        b.Property(x => x.HhFimOcorOld).HasMaxLength(4);
        b.Property(x => x.CdMotOcDefault).HasMaxLength(4);
        b.Property(x => x.CdUsuarioAceito).HasMaxLength(20);
        b.Property(x => x.CdUsuarioAutoriza).HasMaxLength(30);
        b.Property(x => x.CodJustific).HasMaxLength(4);

        // FK interna: freq1(tpocorr, cdmotoc) -> mfre1(tpocorr, cdmotoc)
        b.HasOne<Mfre1>()
         .WithMany()
         .HasForeignKey(x => new { x.TpOcorr, x.CdMotOc })
         .OnDelete(DeleteBehavior.NoAction)
         .HasConstraintName("FK_FREQ1_MFRE1");

        b.HasIndex(x => x.Id);
    }
}
