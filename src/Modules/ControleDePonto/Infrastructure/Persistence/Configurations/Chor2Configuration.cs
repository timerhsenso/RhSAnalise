// Chor2Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Chor2Configuration : IEntityTypeConfiguration<Chor2>
{
    public void Configure(EntityTypeBuilder<Chor2> b)
    {
        b.ToTable("CHOR2");

        b.HasKey(x => new { x.CdCargHor, x.DiaDaSemana });

        b.Property(x => x.CdCargHor).HasMaxLength(2).IsRequired();
        b.Property(x => x.HhEntrada).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhSaida).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhIniInt).HasMaxLength(5).IsRequired();
        b.Property(x => x.HhFimInt).HasMaxLength(5).IsRequired();

        b.Property(x => x.CodHors1050).HasMaxLength(30);

        // FK interna: CHOR2 -> CHOR1 (idhorarioadministrativo)
        b.HasOne<Chor1>()
         .WithMany()
         .HasForeignKey(x => x.IdHorarioAdministrativo)
         .OnDelete(DeleteBehavior.NoAction)
         .HasConstraintName("FK_chor2_chor1_idhorarioadministrativo");

        // Índice conforme DDL
        b.HasIndex(x => x.IdHorarioAdministrativo)
         .HasDatabaseName("IX_chor2_idhorarioadministrativo");
    }
}
