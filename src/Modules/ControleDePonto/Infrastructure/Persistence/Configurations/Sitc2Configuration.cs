// Sitc2Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Sitc2Configuration : IEntityTypeConfiguration<Sitc2>
{
    public void Configure(EntityTypeBuilder<Sitc2> b)
    {
        b.ToTable("sitc2");

        b.HasKey(x => x.Id);

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.CdUsuario).HasMaxLength(20);

        // Índice conforme DDL
        b.HasIndex(x => x.IdFuncionario).HasDatabaseName("IX_sitc2_idfuncionario");
    }
}
