// Test1FreqConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Test1FreqConfiguration : IEntityTypeConfiguration<Test1Freq>
{
    public void Configure(EntityTypeBuilder<Test1Freq> b)
    {
        b.ToTable("test1_freq");

        b.HasKey(x => new { x.CdEmpresa, x.CdFilial });

        b.Property(x => x.DivideExta).HasMaxLength(1);
        b.Property(x => x.CodOcor1).HasMaxLength(4);
        b.Property(x => x.CodOcor2).HasMaxLength(4);
    }
}
