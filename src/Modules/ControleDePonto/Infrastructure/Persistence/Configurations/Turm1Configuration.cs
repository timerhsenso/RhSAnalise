// Turm1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Turm1Configuration : IEntityTypeConfiguration<Turm1>
{
    public void Configure(EntityTypeBuilder<Turm1> b)
    {
        b.ToTable("turm1");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdTurma).HasMaxLength(2).IsRequired();
        b.Property(x => x.DcTurma).HasMaxLength(20);

        // Índice conforme DDL
        b.HasIndex(x => x.IdFilial).HasDatabaseName("IX_turm1_idfilial");
    }
}
