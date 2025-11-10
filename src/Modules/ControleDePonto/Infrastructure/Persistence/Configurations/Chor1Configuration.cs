// Chor1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Chor1Configuration : IEntityTypeConfiguration<Chor1>
{
    public void Configure(EntityTypeBuilder<Chor1> b)
    {
        b.ToTable("chor1");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdCargHor).HasMaxLength(2).IsRequired();
        b.Property(x => x.HhEntrada).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhSaida).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhIniInt).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhFimInt).HasMaxLength(5).IsRequired();
        b.Property(x => x.FlIntervalo).HasMaxLength(1);
        b.Property(x => x.DcCargHor).HasMaxLength(100).IsRequired();
        b.Property(x => x.CodHors1050).HasMaxLength(30);
    }
}
