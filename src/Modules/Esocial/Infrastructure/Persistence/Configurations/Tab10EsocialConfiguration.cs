using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RhSensoERP.Modules.GestaoDePessoas.Core.Entities;

namespace RhSensoERP.Modules.GestaoDePessoas.Infrastructure.Persistence.Configurations;

public class Tab10EsocialConfiguration : IEntityTypeConfiguration<Tab10Esocial>
{
    public void Configure(EntityTypeBuilder<Tab10Esocial> builder)
    {
        builder.ToTable("tab10_esocial");

        builder.HasKey(x => x.Codigo);

        builder.Property(x => x.Codigo)
               .HasColumnName("tab10_codigo")
               .HasColumnType("char(2)")
               .IsRequired();

        builder.Property(x => x.Descricao)
               .HasColumnName("tab10_descricao")
               .HasMaxLength(255)
               .IsRequired();

        builder.Property(x => x.DescDocRequisito)
               .HasColumnName("tab10_desc_doc_requisito")
               .HasMaxLength(255);
    }
}
