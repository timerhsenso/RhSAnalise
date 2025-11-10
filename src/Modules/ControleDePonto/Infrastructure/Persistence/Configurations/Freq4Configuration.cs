// Freq4Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Freq4Configuration : IEntityTypeConfiguration<Freq4>
{
    public void Configure(EntityTypeBuilder<Freq4> b)
    {
        b.ToTable("FREQ4");

        b.HasKey(x => new { x.NoMatric, x.CdEmpresa, x.CdFilial, x.Data });

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.FlIntervalo).HasMaxLength(1);
        b.Property(x => x.InicioOld).HasMaxLength(5);
        b.Property(x => x.FimOld).HasMaxLength(5);
        b.Property(x => x.InicioIntervaloOld).HasMaxLength(5);
        b.Property(x => x.FimIntervaloOld).HasMaxLength(5);
    }
}
