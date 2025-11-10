// GloEventosConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

public sealed class GloEventosConfiguration : IEntityTypeConfiguration<GloEventos>
{
    public void Configure(EntityTypeBuilder<GloEventos> b)
    {
        b.ToTable("GLO_EVENTOS");

        b.HasKey(x => new { x.EntInCodigo, x.EveStNome });

        b.Property(x => x.EveStNome).HasMaxLength(50).IsRequired();
        // FK para GLO_ENTIDADES omitida (outro módulo).
    }
}
