using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class Tab8EsocialConfiguration : IEntityTypeConfiguration<Tab8Esocial>
{
    public void Configure(EntityTypeBuilder<Tab8Esocial> builder)
    {
        builder.ToTable("tab8_esocial");

        builder.HasKey(x => x.Codigo);

        builder.Property(x => x.Codigo)
               .HasColumnName("tab8_codigo")
               .HasColumnType("char(2)")
               .IsRequired();

        builder.Property(x => x.Descricao)
               .HasColumnName("tab8_descricao")
               .HasMaxLength(255)
               .IsRequired();
    }
}
