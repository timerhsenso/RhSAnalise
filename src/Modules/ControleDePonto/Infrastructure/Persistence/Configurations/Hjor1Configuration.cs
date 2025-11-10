// Hjor1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class Hjor1Configuration : IEntityTypeConfiguration<Hjor1>
{
    public void Configure(EntityTypeBuilder<Hjor1> b)
    {
        b.ToTable("hjor1");

        b.HasKey(x => x.Id);

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.TpJornada).HasMaxLength(1);
        b.Property(x => x.CdCargHor).HasMaxLength(2);
        b.Property(x => x.CdUsuario).HasMaxLength(20);
        b.Property(x => x.DcDoc).HasMaxLength(20);
        b.Property(x => x.CdRegime).HasMaxLength(10);
        b.Property(x => x.HEspecial).HasMaxLength(1);

        // Índice único do DDL
        b.HasIndex(x => new { x.DtMudanca, x.NoMatric, x.CdEmpresa, x.CdFilial })
         .IsUnique()
         .HasDatabaseName("hjor1nx1");

        b.HasIndex(x => x.IdFuncionario)
         .HasDatabaseName("IX_hjor1_idfuncionario");
    }
}
