// BancoHorasConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.ControleDePonto.Core.Entities;

namespace RhSensoERP.Modules.ControleDePonto.Core.Configurations;

/// <summary>Configuration for BancoHoras.</summary>
public sealed class BancoHorasConfiguration : IEntityTypeConfiguration<BancoHoras>
{
    public void Configure(EntityTypeBuilder<BancoHoras> b)
    {
        b.ToTable("BancoHoras");

        b.HasKey(x => x.Id);

        b.Property(x => x.NoMatric).HasMaxLength(8).IsRequired();
        b.Property(x => x.DebCred).HasMaxLength(1).IsRequired();
        b.Property(x => x.Tipo).HasMaxLength(1).IsRequired();
        b.Property(x => x.Descricao).HasMaxLength(100);
        b.Property(x => x.CdConta).HasMaxLength(4);

        // Sem FK externa (tcon2) para evitar dependência entre módulos.
        b.HasIndex(x => new { x.CdEmpresa, x.CdFilial, x.NoMatric, x.Data });
    }
}
