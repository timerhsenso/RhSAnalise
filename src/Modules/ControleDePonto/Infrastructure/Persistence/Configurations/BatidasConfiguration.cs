// BatidasConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Infrastructure.Persistence.Configurations;

public sealed class BatidasConfiguration : IEntityTypeConfiguration<Batidas>
{
    public void Configure(EntityTypeBuilder<Batidas> b)
    {
        b.ToTable("BATIDAS");

        b.HasKey(x => new { x.CdEmpresa, x.CdFilial, x.NoMatric, x.Data, x.Hora });

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.Hora).HasMaxLength(5).IsRequired();
        b.Property(x => x.Tipo).HasMaxLength(2).IsRequired();
        b.Property(x => x.Erro).HasMaxLength(10).IsRequired();
        b.Property(x => x.Motivo).HasMaxLength(200);

        b.HasIndex(x => x.IdFuncionario);
    }
}
