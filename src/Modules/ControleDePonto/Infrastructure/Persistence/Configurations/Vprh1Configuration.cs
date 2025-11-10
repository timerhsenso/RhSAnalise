// Vprh1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class Vprh1Configuration : IEntityTypeConfiguration<Vprh1>
{
    public void Configure(EntityTypeBuilder<Vprh1> b)
    {
        b.ToTable("vprh1");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdValor).HasMaxLength(4).IsRequired();
        b.Property(x => x.DcValor).HasMaxLength(100).IsRequired();
    }
}
