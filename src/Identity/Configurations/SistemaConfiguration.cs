using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Configurations;

/// <summary>
/// Mapeia a entidade <see cref="Sistema"/> para a tabela tsistema.
/// Regras principais:
/// - PK: CdSistema (char(10))
/// - Campos: DcSistema (varchar(60)), Ativo (bit, default 1)
/// - Relacionamento: 1:N com Funcao
/// </summary>
public sealed class SistemaConfiguration : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> builder)
    {
        builder.ToTable("tsistema");

        builder.HasKey(e => e.CdSistema);

        builder.Property(e => e.CdSistema)
            .HasColumnName("cdsistema")
            .HasMaxLength(10)
            .IsRequired()
            .IsFixedLength();

        builder.Property(e => e.DcSistema)
            .HasColumnName("dcsistema")
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(e => e.Ativo)
            .HasColumnName("ativo")
            .HasDefaultValue(true);

        builder.HasMany(e => e.Funcoes)
            .WithOne(f => f.Sistema)
            .HasForeignKey(f => f.CdSistema)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
