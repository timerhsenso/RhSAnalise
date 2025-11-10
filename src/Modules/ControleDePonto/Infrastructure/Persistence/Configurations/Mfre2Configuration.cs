// Mfre2Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class Mfre2Configuration : IEntityTypeConfiguration<Mfre2>
{
    public void Configure(EntityTypeBuilder<Mfre2> b)
    {
        b.ToTable("mfre2");

        b.HasKey(x => x.Id);

        // FK interna: mfre2(tpocorr, cdmotoc) -> mfre1(tpocorr, cdmotoc)
        b.HasOne<Mfre1>()
         .WithMany()
         .HasForeignKey(x => new { x.TpOcorr, x.CdMotOc })
         .OnDelete(DeleteBehavior.NoAction)
         .HasConstraintName("PK_MFRE2_MFRE1");
    }
}
