using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class Tab21EsocialConfiguration : IEntityTypeConfiguration<Tab21Esocial>
{
    public void Configure(EntityTypeBuilder<Tab21Esocial> builder)
    {
        builder.ToTable("tab21_esocial");

        builder.HasKey(x => x.Codigo);

        builder.Property(x => x.Codigo)
               .HasColumnName("tab21_codigo")
               .HasColumnType("char(4)")
               .IsRequired();

        builder.Property(x => x.Descricao)
               .HasColumnName("tab21_descricao")
               .HasMaxLength(255)
               .IsRequired();
    }
}
