using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Identity.Domain.Entities;

namespace RhSensoERP.Identity.Infrastructure.Persistence.Configurations;

public sealed class SistemaConfiguration : IEntityTypeConfiguration<Sistema>
{
    public void Configure(EntityTypeBuilder<Sistema> builder)
    {
        builder.ToTable("tsistema");

        builder.HasKey(e => e.CdSistema);

        // ⚠️ IGNORAR propriedades que NÃO existem no banco legado
        builder.Ignore(e => e.Id);
        builder.Ignore(e => e.CreatedAt);
        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedAt);
        builder.Ignore(e => e.UpdatedBy);

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