// Mfre1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Mfre1Configuration : IEntityTypeConfiguration<Mfre1>
{
    public void Configure(EntityTypeBuilder<Mfre1> b)
    {
        b.ToTable("mfre1");

        b.HasKey(x => new { x.TpOcorr, x.CdMotOc });

        b.Property(x => x.CdMotOc).HasMaxLength(4).IsRequired();
        b.Property(x => x.DcMotOc).HasMaxLength(40);
        b.Property(x => x.CdConta).HasMaxLength(4);
        b.Property(x => x.CdMotOcLink).HasMaxLength(4);
    }
}
