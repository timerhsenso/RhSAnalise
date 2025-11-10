// Calnd1Configuration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class Calnd1Configuration : IEntityTypeConfiguration<Calnd1>
{
    public void Configure(EntityTypeBuilder<Calnd1> b)
    {
        b.ToTable("calnd1");

        b.HasKey(x => x.Id);

        b.Property(x => x.CdMunicip).HasMaxLength(5).IsRequired();
        b.Property(x => x.CdFeriado).HasMaxLength(1).IsRequired();

        // Índice conforme DDL
        b.HasIndex(x => x.IdMunicipio).HasDatabaseName("IX_calnd1_idmunicipio");
    }
}
