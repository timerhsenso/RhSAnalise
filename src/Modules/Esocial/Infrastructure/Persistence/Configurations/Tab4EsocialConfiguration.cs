using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.Esocial.Core.Entities;

namespace RhSensoERP.Modules.Esocial.Infrastructure.Persistence.Configurations;

public class Tab4EsocialConfiguration : IEntityTypeConfiguration<Tab4Esocial>
{
    public void Configure(EntityTypeBuilder<Tab4Esocial> builder)
    {
        builder.ToTable("tab4_esocial");

        builder.HasKey(x => x.Codigo);

        builder.Property(x => x.Codigo)
               .HasColumnName("tab4_codigo")
               .HasColumnType("char(3)")
               .IsRequired();

        builder.Property(x => x.Descricao)
               .HasColumnName("tab4_descricao")
               .HasMaxLength(255)
               .IsRequired();
    }
}
