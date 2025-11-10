// Freq2Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Freq2Configuration : IEntityTypeConfiguration<Freq2>
{
    public void Configure(EntityTypeBuilder<Freq2> b)
    {
        b.ToTable("freq2");

        b.HasKey(x => x.Id);

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.InicioOld).HasMaxLength(5);
        b.Property(x => x.FimOld).HasMaxLength(5);

        b.HasIndex(x => new { x.CdEmpresa, x.CdFilial, x.NoMatric, x.Data });
    }
}
