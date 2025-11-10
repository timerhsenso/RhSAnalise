// Ctur1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class Ctur1Configuration : IEntityTypeConfiguration<Ctur1>
{
    public void Configure(EntityTypeBuilder<Ctur1> b)
    {
        b.ToTable("ctur1");

        b.HasNoKey(); // tabela sem PK
        b.Property(x => x.CdTurma).HasMaxLength(2).IsRequired();
        b.Property(x => x.HhEntrada).HasMaxLength(5);
        b.Property(x => x.HhSaida).HasMaxLength(5);
        b.Property(x => x.PontoRepeticao).HasMaxLength(1);
    }
}
